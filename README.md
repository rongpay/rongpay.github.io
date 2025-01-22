# 恭喜发财接口文档(v.250122)
    文档内容最后更新于：2025.01.22
    

    
## <span style="color:red !important;"> 特别注意：</span>
1. **时间戳为秒级，非毫秒级，毫秒级请/1000**
2. **returnUrl/notifyUrl 为完整地址,含有协议+端口。如果回调通知地址（notifyUrl）不传，平台不会发起异步回调，需要调用查询接口确认订单状态。**
3. **金额为整数，非小数，以分为单位，不能包含有“·”号**，例：123 即 1.23 元。
1. **商户编号需要从商户后台首页获取，并非登陆账号**，商户密钥（apikey）每次刷新都会重新随机生成，保存好最后一次刷新的密钥进行对接即可。
1. **商户接收异步通知时，不要写死固定参数接收，请使用通用的json/map 对象接收，这样可接收完整参数，然后对json/map 里面的参数进行签名校验。如果只接收固定参数，会导致签名验证失败。后期如果通知增加参数，也可以不用修改代码**
1. **商户测试时如果要确认能不能回调以及验证签名是否成功，可生成订单后直接取消，取消后系统便会有通知。测试完取消通过后，再联系客服测试成功订单**


### 更新记录
1. 2025.01.22
    1. 代付申请接口新增bankCode参数，非必须，某些代付渠道需要传该参数，具体联系客服
1. 2025.01.20
    1. 代付响应、通知 新增 message 字段
1. 2025.01.04
    1. 新增代付接口
1. 2020.03.23
    1. 新增下发查询接口
1. 2020.03.20
    1. 新增银行卡api下发接口
    2. 修改部分描述
1. 2019.12.20 
    1. 订单状态新增 -20（暂无渠道） ，该状态下无支付信息（支付状态，支付成功时间，支付编号为空）

### 接口规范 
1. 字符编码：UTF-8
1. Content-Type：application/json
1. URL 传输参数需要对参数进行 UrlEncode

### 接口调用所需条件
1. 网关地址：请联系客服
1. 商户编号(merchantNo)
2. 商户密钥(apiKey)

### 签名（sign）算法
```
BCrypt(Base64(SHA-256(apiKey+originalStr+apiKey)))
```
1. originalStr: 除**sign参数外**其他**参数值非空**（空值或者空字符串）的参数按**参数名称字母正序排序**然后以name=UrlEncode(value)形式组合，
通过&拼接不同参数。
    1. **注：空值（空值或者空字符串）不参与签名。**
    2. **注：value需要进行UrlEncode编码**
1. BCrypt(Base64(SHA-256(apiKey+originalStr+apiKey))) 
    1. 用SHA-256算法将“apiKey+originalStr+apiKey”进行签名得到签名信息（二进制,有些工具会生成16进制）
    1. 使用Base64编码对SHA-256二进制签名信息进行编码
    1. 使用BCrypt对编码字符串进行签名得到最终签名字符串
1. [php demo](https://github.com/rongpay/rongpay.github.io/tree/master/php-demo)
2. [java demo](https://github.com/rongpay/rongpay.github.io/tree/master/java-demo)
3. [c# demo](https://github.com/rongpay/rongpay.github.io/tree/master/C%23-demo)

### 同步通知 （returnUrl）
当创建订单时传入返回地址，订单结束后，用户点击“返回商户”，会在返回链接带上参数（returnUrl?urlparams）。参数内容[参考统一返回参数](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0)，可通过签名算法计算签名的正确性。例：
```
returnUrl?
    amount=100&
    payMode=100001&
    ts=1575948756&
    orderStatus=50&
    payNo=20191209194326631108714792&
    payStatus=30&
    payTime=1575948756&
    merchantNo=20191204192421307122140114&
    orderNo=201912081855183951ab02e&
    sign=%242a%2410%24JwOX9nmVHrE6o8vcoSmyd.T69Yl7n322tVLmz.pVkRUz%2f.tRCjELS

```
    
### 异步回调 （notifyUrl）

当创建订单时传入异步回调地址时，订单结束后（订单已完成（50））进行通知，总共通知3次，间格时间分别为0s,15,60s，超时时间为10s，处理成功后返回 *success*，返回其他字符表示处理失败，会继续进行后续通知。通知内容[参考统一返回参数](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0)，可通过签名算法计算签名的正确性
示例：
```
curl -X POST "回调地址"
  -H 'content-type: application/json' 
  -d '{
    "amount":100,
    "payMode":"100001",
    "ts":1575948756,
    "orderStatus":50,
    "payNo":"20191209194326631108714792",
    "payStatus":30,
    "payTime":1575948756,
    "merchantNo":"20191204192421307122140114",
    "orderNo":"201912081855183951ab02e",
    "sign":"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T69Yl7n322tVLmz.pVkRUz/.tRCjELS"
}'
```

### 1. 订单接口内容
1. 创建订单接口
    1. 使用场景：当用户充值时，根据下面参数，商户生成跳转链接，返回给用户进行跳转到支付页面。
    2. 请求方式：页面跳转
    3. 请求地址：网关地址+/pay-order/#/?urlparams
    4. 请求参数 
    
        |参数名称| 必须|数据类型|示例| 参数说明 |
        |  ----  | ------------|---- |---- |------------  |
        |amount| 是|整数|100| 金额,以分为单位；最小值100，即1元 |
        |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
        |name|是|字符串|张三|付款人姓名|
        |orderNo|是|字符串(<50)|201912081855183951ab02e| 商户订单编号|
        |payMode|是|字符串|100001| 支付模式，请登陆商户后台获取|
        |ts|是|整数|1575948756| 商户订单时间戳（秒级）|
        |notifyUrl|否|字符串|https://www.baidu.com/notify| 后台通知地址|
        |returnUrl|否|字符串|https://www.baidu.com| 支付完成用户返回地址|
        |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，请按照签名算法生成|

    6. 响应
    5. 示例
    ```
    网关地址+/pay-order/#/?amount=100&merchantNo=20191204192421307122140114&orderNo=1575730270288&payMode=100001&ts=1575730270&sign=%242a%2410%24JwOX9nmVHrE6o8vcoSmyd.T69Yl7n322tVLmz.pVkRUz%2F.tRCjELS
    ```
2. 查询订单接口
    1. 使用场景：当商户需要对特定订单查询时
    2. 请求方式：POST
    2. 请求地址：网关地址+ /any-pay/open/order/query
    4. 请求参数
    
        |参数名称|必须|数据类型|示例| 参数说明|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
        |orderNo|是|字符串(<50)|201912081855183951ab02e| 商户订单编号|
        |ts|是|整数|1575948756| 商户订单时间戳（秒级）|
        |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，请按照签名算法生成|
    
    6. 响应（[参考统一返回参数](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0)）
    5. 示例
    ```
    请求： curl -X POST "网关地址+/any-pay/open/order/query"  -H  "accept:*/*"  -H  "Content-Type:application/json" -d "{\"merchantNo\":\"20191204192421307122140114\",\"orderNo\":\"201912081855183951ab02e\",\"sign\":\"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T69Yl7n322tVLmz.pVkRUz/.tRCjELS\",\"ts\":1575948756}"
    响应： 
    ```
    
### 统一返回参数 
1. 参数内容
    
    |参数名称|必须|数据类型|示例| 参数说明|
    |  ----  | ------------  |----  |----  |------------  | 
    |amount|是|整数|100| 金额,以分为单位|
    |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
    |orderNo|是|字符串(<50)|201912081855183951ab02e| 商户订单编号|
    |payMode|是|字符串|100001| 支付模式|
    |ts|是|整数|1575948756| 商户订单时间戳（秒级）|
    |orderStatus|是|整数|50| 订单状态，请参考订单状态枚举|
    |payNo|否|字符串|20191209194326631108714792| 支付订单编号|
    |payStatus|否|整数|30| 支付状态，请参考支付状态枚举|
    |payTime|否|整数|1575948756| 支付成功时间（秒级）|
    |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，使用BCrypt校验方法校验| 
    
1. 订单状态（orderStatus）枚举 

    |值|说明|
    |  ----  | ------------  |
    | -20  | 暂无渠道，此状态下无支付状态 |
    | 30  | 支付等待中 |
    | -30  | 用户取消订单 |
    | -40  | 用户支付超时 |
    | -50  | 订单失败 |
    | 50  | 订单已完成 |
    
2. 支付状态（payStatus）枚举 

    |值|说明|
    |  ----  | ------------  | 
    | 10 | 等待支付|
    | -10 | 支付超时 |
    | -20 | 支付取消 |
    | 30 | 支付成功 |
    | -30 | 支付失败 |
        
**以订单状态为主进行判断，支付超时后状态可能会收到支付成功状态通知，请注意处理**
    
### 2. 代付接口
1. 代付申请接口
    1. 使用场景：代付
    2. 请求方式：POST
    2. 请求地址：网关地址+ /any-pay/open/merchant/transfer/apply
    2. **请求头：** X-REQUEST-TOKEN:随机字符串
    4. 请求参数
    
        |参数名称|必须|数据类型|示例| 参数说明|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
        |amount|是|整数|500000| 金额（分）|
        |orderNo|是|字符串|1735965033012| 订单编号|
        |payMode|是|字符串|x2001| 模式编号,联系客服|
        |account|是|字符串|xxxxx| 收款账号|
        |bankCode|否|字符串|ICBC| 银行编码|
        |name|是|字符串|张三| 收款人|
        |notifyUrl|是|字符串|http://www.baidu.com| 通知地址|
        |ts|是|整数|1575948756| 商户订单时间戳（秒级）|
        |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，请按照签名算法生成|
    
    6. 响应:参考[响应内容](https://rongpay.github.io/#代付响应内容)
    5. 示例
    ```
    请求：
    curl --location '网关+/any-pay/open/merchant/transfer/apply' \
        --header 'X-REQUEST-TOKEN: xxxx' \
        --header 'Content-Type: application/json' \
        --data '{"merchantNo":"20241218110252173100554114","ts":1735965033,"sign":"$2a$10$9jreNJCNulcE8a79h99ZBuFUeQPuJUjxNckRXXZLHgZc.hBcBhhsG","amount":1000,"orderNo":"1735965033012","account":"zhangsan","name":"zs","payMode":"x2001","notifyUrl":"http://www.baidu.com"}'
    响应：
    {"merchantNo":"20241218110252173100554114","orderNo":"1735965033012","applyNo":"20250104123046200142181117","amount":1000,"serviceCharge":101,"status":13}
    ```
2. 代付查询接口
    1. 使用场景：代付
    2. 请求方式：POST
    2. 请求地址：网关地址+ /any-pay/open/merchant/transfer/query
    2. **请求头：** X-REQUEST-TOKEN:随机字符串
    4. 请求参数
    
        |参数名称|必须|数据类型|示例| 参数说明|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
        |orderNo|是|字符串|1735965033012| 订单编号|
        |ts|是|整数|1575948756| 商户订单时间戳（秒级）|
        |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，请按照签名算法生成|
    
    6. 响应:参考[响应内容](https://rongpay.github.io/#代付响应内容)
    5. 示例
    ```
    请求：
    curl --location 'https://网关+/any-pay/open/merchant/transfer/query' \
    --header 'Content-Type: application/json' \
    --data '{"merchantNo":"20241218110252173100554114","ts":1735897292,"sign":"$2a$10$WU1OaPTcyP/zfSLPRUW8eeLXQNO824VpDGYwCKGJoktUoLaICA8Z.","orderNo":"1735894585397"}'
    响应：
    {"merchantNo":"20241218110252173100554114","orderNo":"1735965033012","applyNo":"20250104123046200142181117","amount":1000,"serviceCharge":101,"status":13}
    ```
### 代付响应内容
1. 响应参数

    |参数名称|必须|数据类型|示例| 参数说明|
    |  ----  | ------------  |----  |----  |------------  |
    |merchantNo|是|字符串|20241218110252173100554114|商户编号|
    |orderNo|是|字符串|1735965033012| 订单编号|
    |applyNo|是|字符串|201912041924213071221490224| 申请编号|
    |amount|是|整数|500000| 金额（分）|
    |serviceCharge|是|整数|300| 服务费金额（分）|
    |status|是|整数|10| 申请状态|
    |message|否|字符串|代付成功| 代付信息|
    |ts|是|整数|1575948756| 时间戳（秒级）,回调通知时有|
    |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，请按照签名算法生成，回调通知时有|

1. 枚举值
    1. 申请状态（status）枚举 

    |值|说明|
    |  ----  | ------------  | 
    | 10 | 等待处理|
    | 13 | 支付中 |
    | 16 | 待确认 |
    | 20 | 成功 |
    | -20 | 失败 |

1. 示例
    ```
    {"merchantNo":"20241218110252173100554114","orderNo":"1735965033012","applyNo":"20250104123046200142181117","amount":1000,"serviceCharge":101,"status":13,message:null}
    ```
### 代付异步回调 （notifyUrl）

当创建订单时传入异步回调地址时，代付结束后（成功[20]、失败[-20]）进行通知，总共通知3次，间格时间分别为0s,15,60s，超时时间为10s，处理成功后返回 *success*，返回其他字符表示处理失败，会继续进行后续通知。通知内容[参考统一返回参数](https://rongpay.github.io/#代付响应内容)，可通过签名算法计算签名的正确性
示例：
```
curl -X POST "回调地址"
  -H 'content-type: application/json' 
  -d '{"merchantNo":"20241218110252173100554114""orderNo":"1735965033012","applyNo":"20250104123046200142181117","amount":1000,"serviceCharge":101,"status":-20,message:null,"ts":1735965414,"sign":"$2a$10$xaxWqSrekcFwniMfHr460ueXW5LfkmtqkBYqOxGLntu8Bp5pgmLQe"}'
```

### 3. 下发api接口
1. 银行卡下发
    1. 使用场景：商户银行卡下发
    2. 请求方式：POST
    2. 请求地址：网关地址+ /any-pay/open/merchant/withdraw-apply
    2. **请求头：** X-REQUEST-TOKEN:随机字符串
    4. 请求参数
    
        |参数名称|必须|数据类型|示例| 参数说明|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
        |bankName|是|字符串|中国银行| 银行名称|
        |bankcard|是|字符串|6225804598346543| 银行卡号|
        |realName|是|字符串|张三| 持卡人|
        |passwd|是|字符串|MD5(密码)| 提现密码（需要md5）|
        |amt|是|整数(500000-5000000)|500000| 金额（分）|
        |ts|是|整数|1575948756| 商户订单时间戳（秒级）|
        |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，请按照签名算法生成|
    
    6. 响应:参考[响应内容](https://rongpay.github.io/#%E5%93%8D%E5%BA%94%E5%86%85%E5%AE%B9)
    5. 示例
    ```
    请求： curl -X POST "网关+/any-pay/open/merchant/withdraw-apply" -H "X-REQUEST-TOKEN:111111we2324" 
    -H "Content-Type:application/json" 
    -d "{\"amt\":500000,\"bankName\":\"中国银行\",\"bankcard\":\"6225804598346543\",\"merchantNo\":\"20191204192421307122140114\",\"passwd\":\"MD5(密码)\",\"realName\":\"张三\",\"sign\":\"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6\",\"ts\":1575948756}"
    ```
1. 下发查询
    1. 使用场景：商户银行卡下发查询
    2. 请求方式：POST
    2. 请求地址：网关地址+ /any-pay/open/merchant/withdraw-apply/query
    2. **请求头：** X-REQUEST-TOKEN:随机字符串
    4. 请求参数
    
        |参数名称|必须|数据类型|示例| 参数说明|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|是|字符串|20191204192421307122140114| 商户编号|
        |applyNo|是|字符串|201912041924213071221490224| 申请编号|
        |ts|是|整数|1575948756| 商户订单时间戳（秒级）|
        |sign|是|字符串|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| 参数签名，请按照签名算法生成|
    
    6. 响应: 参考[响应内容](https://rongpay.github.io/#%E5%93%8D%E5%BA%94%E5%86%85%E5%AE%B9)
    5. 示例
    ```
    请求： curl -X POST "网关+/any-pay/open/merchant/withdraw-apply/query" 
    -H "X-REQUEST-TOKEN:111111we2324" 
    -H "Content-Type:application/json" 
    -d "{\"merchantNo\":\"20191204192421307122140114\",\"applyNo\":\"201912041924213071221490224\",\"sign\":\"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6\",\"ts\":1575948756}"
    ```

### 响应内容
1. 响应参数

    |参数名称|必须|数据类型|示例| 参数说明|
    |  ----  | ------------  |----  |----  |------------  |
    |type|是|字符串|WITHDRAW|类型|
    |applyNo|是|字符串|201912041924213071221490224| 申请编号|
    |amt|是|整数|500000| 金额（分）|
    |serviceCharge|是|整数|300| 服务费金额（分）|
    |applyStatus|是|整数|10| 申请状态|

1. 枚举值
    1. 申请状态（applyStatus）枚举 

    |值|说明|
    |  ----  | ------------  | 
    | 10 | 等待处理|
    | 13 | 支付中 |
    | 16 | 待确认 |
    | 20 | 成功 |
    | -20 | 失败 |

1. 示例
    ```
    {"applyNo":"20200323111021811157255464","type":"WITHDRAW","amt":500000,"serviceCharge":300,"applyStatus":20}
    ```
    
    
    
