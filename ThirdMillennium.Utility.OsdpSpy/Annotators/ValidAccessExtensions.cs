using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThirdMillennium.Utility.OSDP
{
    public static class ValidAccessExtensions
    {
        public static IEnumerable<ValidAccessItem> CardsSince(
            this IEnumerable<ValidAccessItem> items, 
            DateTime since)
        {
            return items.Where(x => x.IsCard && x.Timestamp > since);
        }

        public static IEnumerable<ValidAccessItem> KeysSince(
            this IEnumerable<ValidAccessItem> items, 
            DateTime since)
        {
            return items.Where(x => !x.IsCard && x.Timestamp > since);
        }

        public static IEnumerable<ValidAccessItem> Until(
            this IEnumerable<ValidAccessItem> items, 
            DateTime until)
        {
            return items.Where(x => x.Timestamp < until);
        }

        public static IEnumerable<string> ToKeyArray(this IEnumerable<ValidAccessItem> items)
        {
            var list = new List<string>();

            foreach (var item in items)
            {
                list.AddRange(item.Payload.ToKeyArray());
            }

            return list;
        }
        
        public static IEnumerable<string> ToKeyArray(this byte[] payload)
        {
            var count = payload[1];
            var result = new string[count];
            for (var i = 0; i < count; ++i)
            {
                result[i] = payload[2 + i].ToKeyString();
            }
            return result;
        }

        public static string ToKeySummary(this IEnumerable<string> keys)
        {
            var builder = new StringBuilder();
            foreach (var key in keys)
            {
                builder.Append($"{key} ");
            }
            return builder.ToString();
        }
    }
}