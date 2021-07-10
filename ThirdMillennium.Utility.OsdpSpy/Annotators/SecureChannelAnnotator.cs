using System;
using System.Linq;
using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class SecureChannelAnnotator : ExchangeAnnotator
    {
        public SecureChannelAnnotator(IKeyStore keys) : base(1)
        {
            _keys = keys;
        }

        private readonly IKeyStore _keys;
        
        private IExchange _input;
        private IAnnotation _output;
        private readonly Session[] _session = new Session[126];

        private readonly byte[] _client = new byte[8];
        private readonly byte[] _rndB = new byte[8];
        private readonly byte[] _rndA = new byte[8];
        private readonly byte[] _clientCryptogram = new byte[16];

        private void OnChallenge()
        {
            // Make sure the security block is selecting a valid key.
            if (_input.Acu.Frame.SecurityBlock[2] > 1) return;
            if (_input.Pd.Frame.SecurityBlock[2] > 1) return;
            
            // Get the data from this exchange.
            Buffer.BlockCopy(_input.Acu.Frame.Data, 0, _rndA, 0, 8);
            Buffer.BlockCopy(_input.Pd.Frame.Data, 0, _client, 0, 8);
            Buffer.BlockCopy(_input.Pd.Frame.Data, 8, _rndB, 0, 8);
            Buffer.BlockCopy(_input.Pd.Frame.Data, 16, _clientCryptogram, 0, 16);
            
            // What secure channel base key are we using?
            var scbk = _input.Acu.Frame.SecurityBlock[2] == 0
                ? _keys.DefaultBaseKey
                : _keys.Find(_client);
            
            // Do we have a valid secure channel base key?
            if (scbk == null) return;
            
            // Create a new session.
            var session = new Session();
            session.Generate(scbk, _rndA, _rndB);

            // Verify the client cryptogram.
            var verified = session.ClientCryptogram.SequenceEqual(_clientCryptogram);

            // Show authentication status and secure channel base key.
            _output.AppendItem("Authenticating", verified);
            
            // Do we have the makings of a session?
            if (verified)
            {
                _session[_input.Acu.Frame.Address] = session;
                _output.AppendItem("SCBK", scbk.ToHexString());
            }
        }

        private void OnServerCryptogram()
        {
            // Make sure we have a secure channel session to work with. 
            var session = _session[_input.Acu.Frame.Address];
            if (session == null) return;
            
            // Verify the server cryptogram.
            var verifiedScrypt = session.ServerCryptogram.SequenceEqual(_input.Acu.Frame.Data);
            
            // Verify the initial RMAC.
            var rmac = session.ServerCryptogram.Encrypt(session.SMac1).Encrypt(session.SMac2);
            var verifiedRmac = rmac.SequenceEqual(_input.Pd.Frame.Data);

            // Was the authentication successful?
            var authenticated = verifiedScrypt && verifiedRmac;
            _output.AppendNewLine();
            _output.AppendItem("Authenticated", authenticated);
            if (authenticated)
            {
                // Authenticated, so set the initial RMAC.
                session.SetInitialRMac(rmac);
            }
            else
            {
                // Authentication failed, or failed to track the authentication.
                _session[_input.Acu.Frame.Address] = null;
            }
        }

        private void OnProcess()
        {
            // Do we have a secure session?
            var session = _session[_input.Acu.Frame.Address];
            if (session == null) return;

            // Process the frames.
            var processed = 
                session.ProcessFrame(_input.Acu) && 
                session.ProcessFrame(_input.Pd);
            
            // Were we successful?
            if (!processed)
            {
                _session[_input.Acu.Frame.Address] = null;
            }
        }

        private void OnKeyset()
        {
            // Process the exchange normally.
            OnProcess();
            
            // Extract and save the key.
            var plain = _input.Acu.Payload.Plain;
            var keyLength = plain[1];
            var key = new byte[16];
            Buffer.BlockCopy(plain, 2, key, 0, keyLength);
            _keys.Store(_client, key);
        }

        public override void Annotate(IExchange input, IAnnotation output)
        {
            // Update the input and output.
            _input = input;
            _output = output;
            
            // Ignore the configuration address for secure channel.
            if (_input.Acu.Frame.Address == Frame.ConfigurationAddress) return;
            
            // Ignore an incomplete frame.
            if (_input.IsNoResponse()) return;

            switch (_input.Acu.Frame.Command)
            {
                case Command.CHLNG:  
                    OnChallenge();        
                    break;
                
                case Command.SCRYPT: 
                    OnServerCryptogram(); 
                    break;
                
                case Command.KEYSET:
                    OnKeyset();
                    break;
                
                default:
                    OnProcess();
                    break;
            }
        }
    }
}