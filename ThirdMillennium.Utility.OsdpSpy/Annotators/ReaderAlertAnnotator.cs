using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public class ReaderAlertAnnotator : AlertingAnnotator<IExchange>, ISecureChannelSink
    {
        public ReaderAlertAnnotator(IFactory<IAnnotation> factory) : base(factory) {}

        private enum State
        {
            PlainText = 0,
            AuthenticatingDefaultScbk,
            AuthenticatingScbk,
            Authenticated
        }

        private const int ReaderCount = 126;
        private readonly State[] _state = new State[126];

        public void OnAuthenticating(int address, bool isDefault, byte[] scbk)
        {
            _state[address] = isDefault
                ? State.AuthenticatingDefaultScbk
                : State.AuthenticatingScbk;
        }

        public void OnAuthenticationSychronised(int address)
        {
            this.CreateOsdpAlert(
                    _state[address] == State.AuthenticatingDefaultScbk
                        ? "Secure Channel Session Established with Default SCBK"
                        : "Secure Channel Session Established with SCBK")
                .AndLogTo(this);

            _state[address] = State.Authenticated;
        }

        public void OnAuthenticationLost(int address)
        {
            _state[address] = State.PlainText;
        }
    }
}