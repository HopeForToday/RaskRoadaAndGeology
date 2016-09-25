using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlAgilityPackDemo1
{
    /// <summary>
    /// Json工具类
    /// </summary>
    public class JsonUtility
    {
        private static JsonUtility _instance = new JsonUtility();

        /// <summary>
        /// 单例
        /// </summary>
        public static JsonUtility Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }
        /// <summary>
        /// Json转一般对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object JsonToObject(string jsonString, object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return serializer.ReadObject(mStream);
        }
        /// <summary>
        /// Json转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T JsonToObject<T>(string json)
        {
                    var ser = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var jsonObject = (T)ser.ReadObject(ms);
            ms.Close();
            return jsonObject;
        }

        /// <summary>
        /// Json转对象列表  这里去掉了最后一条数据 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public IList<T> JsonToObjectList<T>(string json)
        {
           
            json = json.Replace("]", "").Replace("[", "").Replace("},", "|");

            //var regex = new Regex("},{");
            var jsons = json.Split('|');
            var list = new List<T>();
            int count = jsons.Count();
            int i = 0;
            foreach (var item in jsons)
            {
                if (i == count - 1) break;
                i++;
                var temp = item + "}";
                list.Add(JsonToObject<T>(temp));
            }
            return list;
        }

        /// <summary>
        /// 对象转Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string ObjectToJson(object obj)
        {
            var serializer = new DataContractJsonSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                var sb = new StringBuilder();
                sb.Append(Encoding.UTF8.GetString(ms.ToArray()));
                return sb.ToString();
            }
        }

        /// <summary>
        /// 对象列表转Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectList"></param>
        /// <returns></returns>
        public string ObjectListToJson<T>(IList<T> objectList)
        {
            return ObjectListToJson(objectList, "");
        }

        /// <summary>
        /// 对象列表转Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectList"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public string ObjectListToJson<T>(IList<T> objectList, string className)
        {
            var sbResult = new StringBuilder();
            sbResult.Append("{");
            className = string.IsNullOrEmpty(className) ? objectList[0].GetType().Name : className;
            sbResult.Append("\"" + className + "\":[");

            for (var i = 0; i < objectList.Count; i++)
            {
                var item = objectList[i];
                if (i > 0)
                {
                    sbResult.Append(",");
                }
                sbResult.Append(ObjectToJson(item));
            }

            sbResult.Append("]}");
            return sbResult.ToString();
        }
    }
}
