using System;
using System.Diagnostics;
using OsdpSpy.Annotations;
using OsdpSpy.Extensions;

namespace OsdpSpy.Decoders
{
    public static class ByteArrayAnnotationExtensions
    {
        public static IAnnotation AppendByteArray(
            this IAnnotation annotation, 
            string heading,
            byte[] data, 
            int offset = 0,
            int length = -1)
        {
            try
            {
                var len = length == -1 ? data.Length : length;
                var buffer = new byte[len];
                Buffer.BlockCopy(data, offset, buffer, 0, len);
                return annotation.AppendItem(heading, buffer.ToHexString());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return annotation;
            }        
        }
    }
}