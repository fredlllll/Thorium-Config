using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Thorium.Config
{
    public class Config : DynamicObject
    {
        protected Dictionary<string, object> cache = new Dictionary<string, object>();

        public JObject Values { get; protected set; }

        protected Config() { }

        public Config(JObject obj)
        {
            Values = obj;
        }

        public void ClearCache()
        {
            cache.Clear();
        }

        public bool TryGet<T>(string key, out T result, T defaultValue = default(T))
        {
            if(Values.HasValue(key))
            {
                result = Values.Get<T>(key);
                return true;
            }
            result = defaultValue;
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if(cache.TryGetValue(binder.Name, out result)) //try get from cache
            {
                if(binder.ReturnType.Equals(result.GetType())) //type matches
                {
                    return true;
                }
            }

            string jsonName = binder.Name.FirstCharacterToLower();
            if(Values.HasValue(jsonName))
            {
                result = Values.Get(binder.ReturnType, jsonName);
                cache[binder.Name] = result;
                return true;
            }

            //TODO: should i instead throw an exception with a meaningful message?

            return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Values.Properties().Select(x => x.Name.FirstCharacterToUpper());
        }
    }
}
