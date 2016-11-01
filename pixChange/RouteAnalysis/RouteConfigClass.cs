using RoadRaskEvaltionSystem.HelperClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadRaskEvaltionSystem.RouteAnalysis
{
    class RouteConfigClass : IRouteConfig
    {
        //返回空或者空串 代表没有查询到
        private Dictionary<int, string> queryIndexs;
        public Dictionary<int, string> QueryIndexs
        {
            get
            {
                if (queryIndexs == null)
                {
                    queryIndexs = queryAllIndex();
                }
                return queryIndexs;
            }
        }
        public string QueryGoodRouteIndex(int objectID)
        {
            if (queryIndexs == null)
            {
                queryIndexs = queryAllIndex();
                if (queryIndexs == null)
                {
                    throw new Exception("配置文件出错");
                }
            }
            if (queryIndexs.Keys.Contains(objectID))
            {
                return queryIndexs[objectID];
            }
            return null;
        }
        //刷新
        public void Flush()
        {
            queryIndexs = queryAllIndex();
            if (queryIndexs == null)
            {
                throw new Exception("配置文件出错");
            }
        }
        //获取所有路线和公路网点的对应关系 返回Null代表 配置文件出错
        private Dictionary<int, string> queryAllIndex()
        {
            Dictionary<int, string> queryResults = new Dictionary<int, string>();
            string countStr = ConfigHelper.ReadAppConfig("RoadLineCount");
            if (String.IsNullOrEmpty(countStr))
            {
                return null;
            }
            int count;
            if (!int.TryParse(countStr, out count))
            {
                return null;
            }
            for (int i = 1; i <= count; i++)
            {
                string lineValue = i.ToString() + (i + 1).ToString();
                if (!readSingleRouteConfig(queryResults, lineValue))
                {
                    return null;
                }
            }
            return queryResults;
        }
        //读取单个公路网点之间线要素的数据
        private bool readSingleRouteConfig(Dictionary<int, string> queryResults, string lineValue)
        {
            string tempStr = ConfigHelper.ReadAppConfig("RoadLine" + lineValue);
            string[] arrys = tempStr.Split(' ');
            foreach (var value in arrys)
            {
                int numberValue;
                if (int.TryParse(value, out numberValue))
                {
                    queryResults.Add(numberValue, lineValue);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
