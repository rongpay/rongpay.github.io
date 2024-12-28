using System;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Web;
using System.Net;
using BCrypt.Net;
using System.IO;
public class Demo
{
	/**
	* api key
	*/
	public static String API_KEY = "982549aacbc046de97de28bffca9e68b";
	
	
	public static void Main()
	{
		//注1 !!! 所有请求参数格式均使用 首字母小写驼峰格式 eg: orderNo(订单编号), notifyUrl(通知地址), 
		//注2 !!! http|https请使用小写 (错误示例: HTTP, HTTPS) 回调地址正确示例: https://www.baidu.com  回调地址错误示例: HTTPS://www.baidu.com
		
		//创建订单
		//CreateOrder();
		
		//验签
		//Console.WriteLine(checkHash("J9yMyqsbweqKqGk9cqQ/ESv1BdiUDVLXxB5BbF2eck8=", "$2a$10$6qizWcRhDOraGeIPWWE7M.UL0CvvlUxJQwpoZpCq/wD2w9GNfDhd2"));
		
		//查询订单
		QueryOrder();
		
		
		//回调验签 与 查询验签
		CallbackVerify();
	}
	
	
	/**
	 *	创建订单
	 */
	public static void CreateOrder()
	{
		String ts = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000) + "";
		
		SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();
		parameters.Add("orderNo", ts);
		parameters.Add("merchantNo", "20191204192421307122140114");
		parameters.Add("amount", "100");
		parameters.Add("name", "张三");
		parameters.Add("payMode", "100001");
		parameters.Add("ts", ts);
		//http|https请使用小写
		parameters.Add("notifyUrl", "https://www.baidu.com/");
		parameters.Add("returnUrl", "https://www.baidu.com/");
		
		//拼接参数
		String paramsOrder = GetParamsOrder(parameters);
		Console.WriteLine(paramsOrder);
		//签名
		String signStr = Sign(API_KEY, paramsOrder);
		//UrlEncode
		signStr = WebUtility.UrlEncode(signStr);
		
		//请求地址
		String host = "网关地址/pay-order/#/?" + paramsOrder + "&sign=" + signStr;
		Console.WriteLine(host);
	}
	
	
	/*
	 * 查询订单
	 * 回调验签demo
	 * (仅供参考, 实际使用自行修改)
	 */
	public static void QueryOrder()
	{
		String host = "网关地址/any-pay/open/order/query";
		String ts = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000) + "";
		SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();
		parameters.Add("orderNo", "1576061857");
		parameters.Add("merchantNo", "20191204192421307122140114");
		parameters.Add("ts", ts);
		
		//拼接参数
		String paramsOrder = GetParamsOrder(parameters);
		//签名, 此处的sign参数不需要UrlEncode, 只有创建订单的sign参数需要UrlEncode
		String signStr = Sign(API_KEY, paramsOrder);
		parameters.Add("sign", signStr);
		Console.WriteLine(signStr);
		
		//此处为json 数据, 实际使用引入json库 对parameters对象序列化即可
		String jsonData = "{\"orderNo\":\"1576061857\",\"merchantNo\":\"20191204192421307122140114\",\"ts\":\"" + ts + "\",\"sign\":\"" + signStr + "\"}";
		
		var request = (HttpWebRequest)WebRequest.Create(host);
		request.Method = "POST";
		request.ContentType = "application/json;charset=UTF-8";
		var byteData = Encoding.UTF8.GetBytes(jsonData);
		var length = byteData.Length;
		request.ContentLength = length;
		var writer = request.GetRequestStream();
		writer.Write(byteData, 0, length);
		writer.Close();
		var response = (HttpWebResponse)request.GetResponse();
		//查询结果
		var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
		Console.WriteLine(responseString);
	}
	
	
	/*
	 *	回调验签 与 查询验签
	 *	模拟代码, 仅供参考, 实际使用自行修改
	 */ 
	public static void CallbackVerify()
	{
		//模拟查询响应结果 与回调参数验签
		String result = "{\"amount\":100,\"orderNo\":\"1576061857\",\"merchantNo\":\"20191204192421307122140114\",\"ts\":1576061857,\"payNo\":\"20191211185756217129352808\",\"payStatus\":-10,\"payMode\":\"100001\",\"orderStatus\":-40,\"payTime\":null,\"sign\":\"$2a$10$PBDOrNdRIYvWnL8msEjMSeX/4PXSYOjaVkvjc/2QLq2Vc558TjFES\"}";
		//此处模拟回调(查询)响应参数, 实际使用引入json库序列化即可
		SortedDictionary<string, string> resultDictionary = new SortedDictionary<string, string>();
		resultDictionary.Add("amount", "100");
		resultDictionary.Add("orderNo", "1576061857");
		resultDictionary.Add("merchantNo", "20191204192421307122140114");
		resultDictionary.Add("ts", "1576061857");
		resultDictionary.Add("payNo", "20191211185756217129352808");
		resultDictionary.Add("payStatus", "-10");
		resultDictionary.Add("payMode", "100001");
		resultDictionary.Add("orderStatus", "-40");
		resultDictionary.Add("payTime", null);
		resultDictionary.Add("sign", "$2a$10$PBDOrNdRIYvWnL8msEjMSeX/4PXSYOjaVkvjc/2QLq2Vc558TjFES");
		
		//拼接参数
		String resultParam = GetParamsOrder(resultDictionary);
		//获取到 Base64(SHA-256(apiKey + paramsOrder + apiKey)) 字符串
		String resultBase64Str = Sha256AndBase64(API_KEY + resultParam + API_KEY);
		
		//校验
		Console.WriteLine(CheckHash(resultBase64Str, resultDictionary["sign"]));
		
	}
	
	
	
	
	
	
	
	/*
	 * 验签
	 * plainText: 原文(base64后字符串	Base64(SHA-256(apiKey + paramsOrder + apiKey))
	 * hashText : 密文(BCrypt后字符串	BCrypt(Base64(SHA-256(apiKey + paramsOrder + apiKey)))
	 */
	public static Boolean CheckHash(String plainText, String hashText)
	{
		return BCrypt.Net.BCrypt.Verify(plainText, hashText);
	}
	
	
	/*
	 *	先sha256后再执行base64
	 *	Base64(SHA-256(apiKey + paramsOrder + apiKey))
	 */
	public static String Sha256AndBase64(String source)
	{
		//sha256
		byte[] tmpByte = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(source));
   		 //base64
		return Convert.ToBase64String(tmpByte);
	}
	
	/*
	 * 签名
	 */
	public static String Sign(String apiKey, String paramsOrder)
	{
		String signStr = apiKey + paramsOrder + apiKey;
   		 //sha256 &&  base64
		String base64Str = Sha256AndBase64(signStr);
		//BCrypt
		return BCrypt.Net.BCrypt.HashPassword(base64Str);
	}

	/*
	 * 排序拼接
	 */
	public static string GetParamsOrder(IDictionary<string, string> parameters)
	{
		//排序
		IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
		IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

		//拼接
		StringBuilder query = new StringBuilder("");
		while (dem.MoveNext())
		{
			string key = dem.Current.Key;
			string value = dem.Current.Value;
			if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !"sign".Equals(key))
			{
				query.Append(key).Append("=").Append(WebUtility.UrlEncode(value)).Append("&");				
			}
		}
		return query.ToString().Substring(0, query.Length - 1);
	}
}
