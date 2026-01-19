# Congratulations and Wealth API Documentation (v.260107)
    Last Updated: 2026.01.19
    
## <span style="color:red !important;"> Special Notice:</span>
1. **Timestamps are in seconds, not milliseconds. Please divide by 1000 for milliseconds.**
2. **returnUrl/notifyUrl should be complete URLs, including protocol and port. If the notifyUrl callback address is not provided, the platform will not initiate asynchronous callbacks. You will need to use the query interface to confirm the order status.**
3. **Amounts should be integers, not decimals, expressed in cents, and cannot include a "·" symbol, e.g., 123 means 1.23 yuan.**
1. **The merchant number must be obtained from the merchant backend homepage, not the login account. The merchant key (apikey) is regenerated randomly every time it is refreshed; save the last refreshed key for integration.**
1. **When merchants receive asynchronous notifications, do not use fixed parameter reception. Use a universal JSON/map object to receive complete parameters, then perform signature verification on the parameters within the JSON/map. Receiving only fixed parameters can lead to signature verification failure. For future added parameters in notifications, code modifications won't be necessary.**
1. **If merchants wish to confirm callback functionality and signature verification success during testing, they can generate and then cancel an order. A notification will be sent upon cancellation. After passing the cancellation test successfully, contact customer service for a successful order test.**

### Update Records
1. 2026.01.19
    1. Added non-mandatory parameter (payer's email) for recharge orders.
1. 2026.01.07
    1. Added balance query interface.
1. 2025.01.22
    1. Added non-mandatory bankCode parameter to the payment application interface; some payment channels require this, contact customer service for details.
1. 2025.01.20
    1. Added message field to payment response and notification.
1. 2025.01.04
    1. Added payment interface.
1. 2020.03.23
    1. Added disbursement query interface.
1. 2020.03.20
    1. Added bank card API disbursement interface.
    2. Modified some descriptions.
1. 2019.12.20
    1. Added new order status -20 (No channel available). In this state, there will be no payment information (payment status, payment success time, and payment number will be empty).

### API Specifications
1. Character Encoding: UTF-8
1. Content-Type: application/json
1. URL transmitted parameters need to be URL encoded

### Conditions Required for Interface Calls
1. Gateway Address: Contact customer service
1. Merchant Number (merchantNo)
2. Merchant Key (apiKey)

### Signature (sign) Algorithm
```
BCrypt(Base64(SHA-256(apiKey+originalStr+apiKey)))
```
1. originalStr: All **non-empty non-sign parameters** (parameters with empty values or empty strings excluded) are sorted in ascending order by their names. Parameters are combined in the form of name=UrlEncode(value) and concatenated via &.
    1. **Note: Empty values (empty or empty strings) are not included in the signature.**
    2. **Note: The value needs to be URL encoded.**
1. BCrypt(Base64(SHA-256(apiKey+originalStr+apiKey)))
    1. Use the SHA-256 algorithm to sign "apiKey+originalStr+apiKey" to obtain the signing information (binary; some tools generate hexadecimal).
    1. Use Base64 encoding on the SHA-256 binary signing information.
    1. Use BCrypt to sign the encoded string to get the final signature string.
1. [PHP demo](https://github.com/rongpay/rongpay.github.io/tree/master/php-demo)
2. [Java demo](https://github.com/rongpay/rongpay.github.io/tree/master/java-demo)
3. [C# demo](https://github.com/rongpay/rongpay.github.io/tree/master/C%23-demo)

### Synchronous Notification (returnUrl)
When creating an order with a return address, users can click "return to merchant" after the order ends. The return link will carry parameters (returnUrl?urlparams). Reference the [Unified Return Parameters](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0). You can verify the correctness of the signature by using the signature algorithm. Example:
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
    
### Asynchronous Callback (notifyUrl)

Upon providing an asynchronous callback address during order creation, a notification will be sent after the order is completed (status 50). It will send a total of 3 notifications at intervals of 0s, 15s, and 60s, with a timeout of 10s. If processed successfully, return *success*; returning any other character indicates processing failure, and subsequent notifications will be sent. Notification content is [referenced in the Unified Return Parameters](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0) and can be verified using the signature algorithm.
Example:
```
curl -X POST "callback address"
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

### 1. Order Interface Content
1. Create Order Interface
    1. Usage Scenario: When users recharge, merchants generate a redirect link based on the parameters below and return it to the user to redirect to the payment page.
    2. Request Method: Page Redirect
    3. Request URL: Gateway Address + /pay-order/#/?urlparams
    4. Request Parameters 
    
        |Parameter Name| Required|Data Type|Example| Description |
        |  ----  | ------------|---- |---- |------------  |
        |amount| Yes|Integer|100| Amount in cents; min value is 100, i.e., 1 yuan. |
        |merchantNo|Yes|String|20191204192421307122140114| Merchant number.|
        |name|Yes|String|Zhang San|Payer's name.|
        |email|No|String|zhamgsan@gmeil.com|Payer's email.|
        |orderNo|Yes|String(<50)|201912081855183951ab02e| Merchant order number.|
        |payMode|Yes|String|100001| Payment mode. Log in to the merchant backend to obtain.|
        |ts|Yes|Integer|1575948756| Merchant order timestamp (seconds level).|
        |notifyUrl|No|String|https://www.baidu.com/notify| Background notification address.|
        |returnUrl|No|String|https://www.baidu.com| Return address after payment completion.|
        |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature, please generate according to the signature algorithm.|

    6. Response
    5. Example
    ```
    Gateway Address + /pay-order/#/?amount=100&merchantNo=20191204192421307122140114&orderNo=1575730270288&payMode=100001&ts=1575730270&sign=%242a%2410%24JwOX9nmVHrE6o8vcoSmyd.T69Yl7n322tVLmz.pVkRUz%2F.tRCjELS
    ```
2. Query Order Interface
    1. Usage Scenario: When merchants need to query a specific order.
    2. Request Method: POST
    2. Request URL: Gateway Address + /any-pay/open/order/query
    4. Request Parameters
    
        |Parameter Name|Required|Data Type|Example| Description|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|Yes|String|20191204192421307122140114| Merchant number.|
        |orderNo|Yes|String(<50)|201912081855183951ab02e| Merchant order number.|
        |ts|Yes|Integer|1575948756| Merchant order timestamp (seconds level).|
        |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature, please generate according to the signature algorithm.|
    
    6. Response ([Refer to Unified Return Parameters](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0))
    5. Example
    ```
    Request: curl -X POST "Gateway Address + /any-pay/open/order/query"  -H  "accept:*/*"  -H  "Content-Type:application/json" -d "{\"merchantNo\":\"20191204192421307122140114\",\"orderNo\":\"201912081855183951ab02e\",\"sign\":\"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T69Yl7n322tVLmz.pVkRUz/.tRCjELS\",\"ts\":1575948756}"
    Response: 
    ```
    
### Unified Return Parameters 
1. Parameter Details
    
    |Parameter Name|Required|Data Type|Example| Description|
    |  ----  | ------------  |----  |----  |------------  | 
    |amount|Yes|Integer|100| Amount, in cents.|
    |merchantNo|Yes|String|20191204192421307122140114| Merchant number.|
    |orderNo|Yes|String(<50)|201912081855183951ab02e| Merchant order number.|
    |payMode|Yes|String|100001| Payment mode.|
    |ts|Yes|Integer|1575948756| Merchant order timestamp (seconds level).|
    |orderStatus|Yes|Integer|50| Order status, refer to order status enums.|
    |payNo|No|String|20191209194326631108714792| Payment order number.|
    |payStatus|No|Integer|30| Payment status, refer to payment status enums.|
    |payTime|No|Integer|1575948756| Payment success timestamp (seconds level).|
    |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature; verify using Bcrypt.|

1. Order Status (orderStatus) Enum 

    |Value|Description|
    |  ----  | ------------  |
    | -20  | No channel available, no payment information under this status. |
    | 30  | Payment waiting. |
    | -30  | User cancels the order. |
    | -40  | User payment timeout. |
    | -50  | Order failed. |
    | 50  | Order completed. |
    
2. Payment Status (payStatus) Enum 

    |Value|Description|
    |  ----  | ------------  | 
    | 10 | Waiting for payment.|
    | -10 | Payment timeout. |
    | -20 | Payment canceled. |
    | 30 | Payment successful. |
    | -30 | Payment failed. |
        
**Use order status as the main criterion for judgment. After a payment timeout, you may receive a payment success notification; please handle accordingly.**
    
### 2. Disbursement Interface
1. Disbursement Application Interface
    1. Usage Scenario: Disbursement
    2. Request Method: POST
    2. Request URL: Gateway Address + /any-pay/open/merchant/transfer/apply
    2. **Request Header:** X-REQUEST-TOKEN: Random string
    4. Request Parameters
    
        |Parameter Name|Required|Data Type|Example| Description|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|Yes|String|20191204192421307122140114| Merchant number.|
        |amount|Yes|Integer|500000| Amount (in cents).|
        |orderNo|Yes|String|1735965033012| Order number.|
        |payMode|Yes|String|x2001| Mode number, contact customer service.|
        |account|Yes|String|xxxxx| Account number of the receiver.|
        |bankCode|No|String|ICBC| [Bank code](https://rongpay.github.io/BANK_CODE.json) |
        |bankBranch|No|String|West Road Branch|Corresponding bank branch.|
        |name|Yes|String|Zhang San| Receiver's name.|
        |notifyUrl|Yes|String|http://www.baidu.com| Notification address.|
        |ts|Yes|Integer|1575948756| Merchant order timestamp (seconds level).|
        |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature, please generate according to the signature algorithm.|
    
    6. Response: See [Disbursement Response Content](https://rongpay.github.io/#代付响应内容)
    5. Example
    ```
    Request:
    curl --location 'Gateway +/any-pay/open/merchant/transfer/apply' \
        --header 'X-REQUEST-TOKEN: xxxx' \
        --header 'Content-Type: application/json' \
        --data '{"merchantNo":"20241218110252173100554114","ts":1735965033,"sign":"$2a$10$9jreNJCNulcE8a79h99ZBuFUeQPuJUjxNckRXXZLHgZc.hBcBhhsG","amount":1000,"orderNo":"1735965033012","account":"zhangsan","name":"zs","payMode":"x2001","notifyUrl":"http://www.baidu.com"}'
    Response:
    {"merchantNo":"20241218110252173100554114","orderNo":"1735965033012","applyNo":"20250104123046200142181117","amount":1000,"serviceCharge":101,"status":13}
    ```
2. Disbursement Query Interface
    1. Usage Scenario: Disbursement
    2. Request Method: POST
    2. Request URL: Gateway Address + /any-pay/open/merchant/transfer/query
    2. **Request Header:** X-REQUEST-TOKEN: Random string
    4. Request Parameters
    
        |Parameter Name|Required|Data Type|Example| Description|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|Yes|String|20191204192421307122140114| Merchant number.|
        |orderNo|Yes|String|1735965033012| Order number.|
        |ts|Yes|Integer|1575948756| Merchant order timestamp (seconds level).|
        |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature, please generate according to the signature algorithm.|
    
    6. Response: See [Disbursement Response Content](https://rongpay.github.io/#代付响应内容)
    5. Example
    ```
    Request:
    curl --location 'https://Gateway+/any-pay/open/merchant/transfer/query' \
    --header 'Content-Type: application/json' \
    --data '{"merchantNo":"20241218110252173100554114","ts":1735897292,"sign":"$2a$10$WU1OaPTcyP/zfSLPRUW8eeLXQNO824VpDGYwCKGJoktUoLaICA8Z.","orderNo":"1735894585397"}'
    Response:
    {"merchantNo":"20241218110252173100554114","orderNo":"1735965033012","applyNo":"20250104123046200142181117","amount":1000,"serviceCharge":101,"status":13}
    ```

3. Balance Query Interface
    1. Usage Scenario: Balance Query
    2. Request Method: POST
    2. Request URL: Gateway Address + /any-pay/open/merchant/account/balance
    2. **Request Header:** X-REQUEST-TOKEN: Random string
    4. Request Parameters
    
        |Parameter Name|Required|Data Type|Example| Description|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|Yes|String|20191204192421307122140114| Merchant number.|
        |ts|Yes|Integer|1575948756| Merchant order timestamp (seconds level).|
        |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature, please generate according to the signature algorithm.|
    
    6. Response Parameters

        |Parameter Name|Required|Data Type|Example| Description|
        |  ----  | ------------  |----  |----  |------------  |
        |balance|Yes|Numeric|100.00| Available balance.|
        |frozenAmt|Yes|Numeric|100.00| Frozen amount.|
        
    5. Example
    ```
    Request:
    curl --location 'https://Gateway+/any-pay/open/merchant/account/balance' \
    --header 'Content-Type: application/json' \
    --data '{"merchantNo":"20241218110252173100554114","ts":1735897292,"sign":"$2a$10$WU1OaPTcyP/zfSLPRUW8eeLXQNO824VpDGYwCKGJoktUoLaICA8Z."}'
    Response:
    {"balance":100.00,"frozenAmt":100.00}
    ```

    
### Disbursement Response Content
1. Response Parameters

    |Parameter Name|Required|Data Type|Example| Description|
    |  ----  | ------------  |----  |----  |------------  |
    |merchantNo|Yes|String|20241218110252173100554114|Merchant number.|
    |orderNo|Yes|String|1735965033012| Order number.|
    |applyNo|Yes|String|201912041924213071221490224| Application number.|
    |amount|Yes|Integer|500000| Amount (in cents).|
    |serviceCharge|Yes|Integer|300| Service charge (in cents).|
    |status|Yes|Integer|10| Application status.|
    |message|No|String|Disbursement successful| Disbursement message.|
    |ts|Yes|Integer|1575948756| Timestamp (seconds level), provided in callback notifications.|
    |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature, please generate according to the signature algorithm, provided in callback notifications.|

1. Enum Values
    1. Application Status (status) Enum

    |Value|Description|
    |  ----  | ------------  | 
    | 10 | Waiting for processing|
    | 13 | In payment process |
    | 16 | Awaiting confirmation |
    | 20 | Successful |
    | -20 | Failed |

1. Example
    ```
    {"merchantNo":"20241218110252173100554114","orderNo":"1735965033012","applyNo":"20250104123046200142181117","amount":1000,"serviceCharge":101,"status":13,message:null}
    ```
### Disbursement Asynchronous Callback (notifyUrl)

When an asynchronous callback address is provided during order creation, a notification will be sent after the completion of disbursement (successful [20], failed [-20]). It will send a total of 3 notifications at intervals of 0s, 15s, and 60s, with a timeout of 10s. If processed successfully, return *success*; returning any other character indicates processing failure, and subsequent notifications will be sent. Notification content is referenced in [Unified Return Parameters](https://rongpay.github.io/#代付响应内容) and can be verified using the signature algorithm.
Example:
```
curl -X POST "callback address"
  -H 'content-type: application/json' 
  -d '{"merchantNo":"20241218110252173100554114""orderNo":"1735965033012","applyNo":"20250104123046200142181117","amount":1000,"serviceCharge":101,"status":-20,message:null,"ts":1735965414,"sign":"$2a$10$xaxWqSrekcFwniMfHr460ueXW5LfkmtqkBYqOxGLntu8Bp5pgmLQe"}'
```

### 3. Issuance API Interface
1. Bank Card Issuance
    1. Usage Scenario: Merchant bank card issuance
    2. Request Method: POST
    2. Request URL: Gateway Address + /any-pay/open/merchant/withdraw-apply
    2. **Request Header:** X-REQUEST-TOKEN: Random string
    4. Request Parameters
    
        |Parameter Name|Required|Data Type|Example| Description|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|Yes|String|20191204192421307122140114| Merchant number.|
        |bankName|Yes|String|Bank of China| Bank name.|
        |bankcard|Yes|String|6225804598346543| Bank card number.|
        |realName|Yes|String|Zhang San| Cardholder name.|
        |passwd|Yes|String|MD5(password)| Withdrawal password (needs MD5 encryption).|
        |amt|Yes|Integer(500000-5000000)|500000| Amount (in cents).|
        |ts|Yes|Integer|1575948756| Merchant order timestamp (seconds level).|
        |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature, please generate according to the signature algorithm.|
    
    6. Response: See [Response Content](https://rongpay.github.io/#%E5%93%8D%E5%BA%94%E5%86%85%E5%AE%B9)
    5. Example
    ```
    Request: curl -X POST "Gateway+/any-pay/open/merchant/withdraw-apply" -H "X-REQUEST-TOKEN:111111we2324" 
    -H "Content-Type:application/json" 
    -d "{\"amt\":500000,\"bankName\":\"Bank of China\",\"bankcard\":\"6225804598346543\",\"merchantNo\":\"20191204192421307122140114\",\"passwd\":\"MD5(password)\",\"realName\":\"Zhang San\",\"sign\":\"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6\",\"ts\":1575948756}"
    ```
1. Issuance Query
    1. Usage Scenario: Merchant bank card issuance query.
    2. Request Method: POST
    2. Request URL: Gateway Address + /any-pay/open/merchant/withdraw-apply/query
    2. **Request Header:** X-REQUEST-TOKEN: Random string
    4. Request Parameters
    
        |Parameter Name|Required|Data Type|Example| Description|
        |  ----  | ------------  |----  |----  |------------  |
        |merchantNo|Yes|String|20191204192421307122140114| Merchant number.|
        |applyNo|Yes|String|201912041924213071221490224| Application number.|
        |ts|Yes|Integer|1575948756| Merchant order timestamp (seconds level).|
        |sign|Yes|String|$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6...| Parameter signature, please generate according to the signature algorithm.|
    
    6. Response: Refer to [Response Content](https://rongpay.github.io/#%E5%93%8D%E5%BA%94%E5%86%85%E5%AE%B9)
    5. Example
    ```
    Request: curl -X POST "Gateway+/any-pay/open/merchant/withdraw-apply/query" 
    -H "X-REQUEST-TOKEN:111111we2324" 
    -H "Content-Type:application/json" 
    -d "{\"merchantNo\":\"20191204192421307122140114\",\"applyNo\":\"201912041924213071221490224\",\"sign\":\"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6\",\"ts\":1575948756}"
    ```

### Response Content
1. Response Parameters

    |Parameter Name|Required|Data Type|Example| Description|
    |  ----  | ------------  |----  |----  |------------  |
    |type|Yes|String|WITHDRAW|Type|
    |applyNo|Yes|String|201912041924213071221490224| Application number.|
    |amt|Yes|Integer|500000| Amount (in cents).|
    |serviceCharge|Yes|Integer|300| Service charge (in cents).|
    |applyStatus|Yes|Integer|10| Application status.|

1. Enum Values
    1. Application Status (applyStatus) Enum 

    |Value|Description|
    |  ----  | ------------  | 
    | 10 | Waiting for processing|
    | 13 | In payment process |
    | 16 | Awaiting confirmation |
    | 20 | Successful |
    | -20 | Failed |

1. Example
    ```
    {"applyNo":"20200323111021811157255464","type":"WITHDRAW","amt":500000,"serviceCharge":300,"applyStatus":20}
    ```
