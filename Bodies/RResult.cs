using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LunaNetCore.Bodies.ParasitismBundle;

namespace LunaNetCore.Bodies
{
    /// <summary>
    /// 响应体
    /// </summary>
    public class RResult
    {
        string url;
        HttpMethod Method;
        string resd;
        Bundle rbnd;

        /// <summary>
        /// 构造一个请求返回实例
        /// </summary>
        /// <param name="u">请求URL</param>
        /// <param name="m">请求所使用的方式</param>
        /// <param name="r">服务器响应数据</param>
        /// <param name="r_bnd">寄生方法</param>
        public RResult(string u,HttpMethod m,string r,Bundle r_bnd)
        {
            url = u;
            Method = m;
            resd = r;
            rbnd = r_bnd;
        }

        /// <summary>
        /// 获取请求的URL
        /// </summary>
        public string URL
        {
            get
            {
                return url;
            }
        }

        /// <summary>
        /// 获取请求使用的方法
        /// </summary>
        public HttpMethod RequestMethod
        {
            get
            {
                return Method;
            }
        }

        /// <summary>
        /// 请求的结果（已过时，请使用<see cref="ResultBundle"/>）
        /// </summary>
        public string ResultData
        {
            get
            {
                return resd;
            }
        }

        /// <summary>
        /// 执行回调方法，处理结果数据（推荐）
        /// </summary>
        public Bundle ResultBundle
        {
            get
            {
                return rbnd;
            }
        }
    }
}
