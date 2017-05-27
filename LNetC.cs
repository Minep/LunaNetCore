using LunaNetCore.Bodies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace LunaNetCore
{
    /// <summary>
    /// LNetC核心
    /// </summary>
    public class LNetC
    {
        /// <summary>
        /// 声明HttpRequesting委托
        /// </summary>
        /// <param name="RequestID">请求体ID</param>
        public delegate void HttpRequesting(string RequestID);
        /// <summary>
        /// 声明HttpResponded委托
        /// </summary>
        /// <param name="RequestID">请求体ID</param>
        /// <param name="rrs">响应体</param>
        public delegate void HttpResponded(string RequestID, RResult rrs);
        /// <summary>
        /// 声明AllQueueRequestCompletely委托
        /// </summary>
        public delegate void AllQueueRequestCompletely();
        /// <summary>
        /// 声明ErrorOccursSecondDelegate委托
        /// </summary>
        /// <param name="e">异常</param>
        public delegate void ErrorOccursSecondDelegate(Exception e);

        /// <summary>
        /// 声明请求超时委托
        /// </summary>
        public delegate void HttpTimeOut();

        /* Luna net core callback Event */
        /// <summary>
        /// Http正在请求触发事件
        /// </summary>
        public static event HttpRequesting OnHttpRequesting;
        /// <summary>
        /// Http远程服务器响应完成触发事件
        /// </summary>
        public static event HttpResponded OnHttpResponded;
        /// <summary>
        /// 队列全部请求完毕触发事件
        /// </summary>
        public static event AllQueueRequestCompletely OnAllQueueRequestCompletely;
        /// <summary>
        /// 深层异常事件触发
        /// </summary>
        public static event ErrorOccursSecondDelegate OnErrorOccursInDeepLayer;
        /// <summary>
        /// 请求超时异常触发
        /// </summary>
        public static event HttpTimeOut OnHttpTimeOut;
        /* ----- Intrnal Decleration ----- */

        /// <summary>
        /// Http请求缓冲区
        /// </summary>
        static IDictionary<string, RBody> HttpRequestBuffer;

        /// <summary>
        /// 初始化LNC，请务最优先调用
        /// </summary>
        public static void InitializeLunaNetCore()
        {
            HttpHelper.OnErrorOccurs += (e) => OnErrorOccursInDeepLayer(e);
            HttpHelper.OnRequestTimeOut += () => OnHttpTimeOut();
            HttpRequestBuffer = new Dictionary<string, RBody>();
        }

        /// <summary>
        /// 将请求体添加至请求列队
        /// </summary>
        /// <param name="b">请求体<see cref="RBody"/></param>
        /// <param name="id">请求体ID</param>
        public static void AddRequestBody(RBody b,string id)
        {
            HttpRequestBuffer.Add(id, b);
        }

        /// <summary>
        /// 将请求体从请求列队中移除
        /// </summary>
        /// <param name="id">请求体ID</param>
        /// <returns>是否成功移除</returns>
        public static bool RemoveBody(string id)
        {
            return HttpRequestBuffer.Remove(id);
        }

        /// <summary>
        /// 获取请求体
        /// </summary>
        /// <param name="id">请求体ID</param>
        /// <returns>请求体<see cref="RBody"/></returns>
        public static RBody GetBody(string id)
        {
            return HttpRequestBuffer[id];
        }

        /// <summary>
        /// 开始异步请求
        /// <para>
        /// 注意，该方法将会在另一个线程执行，无需再另外定义线程。
        /// </para>
        /// <para>
        /// 同时，为了保证更好的效果以及正常接收结果，请务必绑定以下事件
        /// </para>
        /// <see cref="OnHttpRequesting"/>
        /// <see cref="OnHttpResponded"/>
        /// <para>
        /// 这里是一些用于拓展的可选事件
        /// </para>
        /// <see cref="OnAllQueueRequestCompletely"/>
        /// <see cref="OnErrorOccursInDeepLayer"/>
        /// </summary>
        public static void StartRequestAsyn()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                foreach (var v in HttpRequestBuffer)
                {
                    RBody rb = v.Value;
                    string rs="";
                    HttpWebResponse wr = null;
                   OnHttpRequesting(v.Key);
                    if (rb.RequestMethod == HttpMethod.GET)
                    {
                        wr = HttpHelper.CreateGetHttpResponse(rb.URL + rb.PatchParameter(), 5000, rb.RequestCookie);
                        if(wr != null) rs = HttpHelper.GetResponseString(wr);
                    }
                    else
                    {
                        wr = HttpHelper.CreatePostHttpResponse(rb.URL, rb.RequestParameter, 5000, rb.RequestCookie);
                        if (wr != null) rs = HttpHelper.GetResponseString(wr);
                    }
                    if (rb.BodyBundle != null) rb.BodyBundle.Invoke(rs);
                    if (rs != "") OnHttpResponded(v.Key, new RResult(rb.URL, rb.RequestMethod, rs, rb.BodyBundle));
                }
                HttpRequestBuffer.Clear();
                OnAllQueueRequestCompletely();
            }));
            t.Start();
        }
    }
}
