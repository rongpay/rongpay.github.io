# 接口文档(v.200610)
    文档内容最后更新于：2020-06-10



### 1. 订单接口内容
1. 创建订单接口
    1. 使用场景：当用户充值时，根据下面参数，请求接口，接口返回数据，商户自定义页面展示。
    3. 请求地址：网关地址(域名部分)+/any-pay/open/api/order
    2. 请求方式：POST
    2. Content-Type：application/json
    4. 请求参数 
    
        |参数名称| 必须|数据类型|示例| 参数说明 |
        |  ----  | ------------|---- |---- |------------  |
        |amount| 是|整数|100| 金额,以分为单位；最小值100，即1元 |
        |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
        |orderNo|是|字符串(<50)|201912081855183951ab02e| 商户订单编号|
        |payMode|是|字符串|100001| 支付模式，请登陆商户后台获取|
        |ts|是|整数|1575948756| 商户订单时间戳（秒级）|
        |ip|是|整数|2130706433| 客户真实ip|
        |notifyUrl|否|字符串|https://www.baidu.com/notify| 后台通知地址|
        |returnUrl|否|字符串|https://www.baidu.com| 支付完成用户返回地址|
        |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，请按照签名算法生成|

    5. 响应参数
        
        |参数名称| 必须|数据类型|示例| 参数说明 |
        |  ----  | ------------|---- |---- |------------  |
        |orderNo|是|字符串(<50)|201912081855183951ab02e| 平台订单编号|
        |amount| 是|整数|30000| 发起金额,以分为单位；最小值100，即1元 |
        |realAmount| 是|整数|29998| 实际支付金额,以分为单位 |
        |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
        |mcntOrderNo|是|字符串(<50)|201912081855183951ab02e| 商户订单编号|
        |mcntOrderTs|是|整数|1591773393000| 商户订单时间戳（毫秒级）|
        |expirTime|是|整数|1591774002282| 订单过期时间（毫秒级）|
        |merchantName|是|字符串|20191204192421307122140114| 商户名称|
        |orderStatus|是|整数|50| 订单状态，请参考订单状态枚举|
        |payNo|是|字符串|20191209194326631108714792| 支付订单编号|
        |payStatus|是|整数|30| 支付状态，请参考支付状态枚举|
        |payChannel|是|对象|{}| 不同模式内容不一样，[请参考](https://github.com/rongpay/rongpay.github.io/tree/rongpay-api/other#2-paychannel-%E5%93%8D%E5%BA%94%E5%86%85%E5%AE%B9)|
        
    5. 示例
    ```
        //请求
        POST /any-pay/open/api/order HTTP/1.1
        Host: 网关地址
        Content-Type: application/json
        Cache-Control: no-cache

        {
            "amount": 30000,
            "ip": 2130706433,
            "merchantNo": "20191204192421307122140114",
            "notifyUrl": "https://www.baidu.com/notify",
            "orderNo": "201912081855183951ab02e",
            "payMode": "100007",
            "returnUrl": "",
            "sign": "$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...",
            "ts": 1575948756
        }
        
        //响应
        {
            "orderNo":"20200610151642293181035065",
            "amount":30000,
            "realAmount":29998,
            "merchantNo":"20191123161925299103193198",
            "mcntOrderNo":"c5c5d727-e45e-4e78-bdc4-f1c3f22648cc",
            "mcntOrderTs":1591773393000,
            "expirTime":1591774002282,
            "merchantName":"商户名称",
            "payChannel":{
                "bankName":"中国银行",
                "realName":"张三",
                "bankcard":"111111111111"
            },
            "payStatus":10,
            "payNo":"20200610151642939132182091",
            "orderStatus":30
        }

    ```
### 2. payChannel 响应内容 
1. 银行卡内容
    |参数名称| 必须|数据类型|示例| 参数说明 |
    |  ----  | ------------|---- |---- |------------  |
    |bankName|是|字符串|中国银行| 银行名称|
    |realName|是|字符串|张三| 持卡人名称 |
    |bankcard|是|字符串|111111111111| 卡号 |

1. 二维码内容
    |参数名称| 必须|数据类型|示例| 参数说明 |
    |  ----  | ------------|---- |---- |------------  |
    |nickName|是|字符串|张三的昵称| 昵称|
    |realName|是|字符串|张三| 真实名称 |
    |qrCode|是|字符串|qrcode| 二维码字符串 |
    |account|否|字符串|张三的账户| 账户|
