# Congratulations Pay API Documentation (v.250104)
    Last Updated: 2025.01.04

---

## <span style="color:red !important;"> IMPORTANT NOTICE: </span>
1. **Timestamps are in seconds, not milliseconds. If milliseconds are used, divide by 1000.**
2. **`returnUrl`/`notifyUrl` should be complete URLs, including protocol and port. If the callback notification address (`notifyUrl`) is not provided, the platform will not send an asynchronous callback. You will need to manually call the query interface to confirm the order status.**
3. **Amounts are integers, not decimals, in unit of cents. Decimal points (e.g., `·`) are not allowed.** Example: 123 means 1.23 CNY.
4. **Merchant Numbers (merchantNo) should be obtained from the merchant dashboard homepage, not the login account. The API key (`apiKey`) is randomly regenerated each time it is refreshed. Save the last refreshed API key for integration.**
5. **When handling asynchronous notifications, merchants must avoid hardcoding fixed parameters for reception. Use a generic JSON/map object instead, so that all parameters can be received completely. Then, validate the signature within the JSON/map object. Hardcoding parameters may cause signature validation failures if new parameters are added in future updates.**
6. **During testing, merchants can create an order and then cancel it to confirm that callbacks work and validate the signature. Only initiate success-order testing after cancel-order testing passes successfully. Contact support for further testing assistance.**

---

### Update Log
1. **2025.01.04**
   - Added Payout/Disbursement API.
2. **2020.03.23**
   - Added Withdrawal Query API.
3. **2020.03.20**
   - Added Bank Transfer API.
   - Updated parameter descriptions.
4. **2019.12.20**
   - Added order status -20 ("No Available Channels"). In this status, payment information (payment status, success time, payment number) will be null.

---

### Interface Specifications
1. Character Encoding: UTF-8
2. Content-Type: application/json
3. URL Parameters must be URL Encoded.

---

### Pre-requisites for API Integration
1. **Gateway Address:** Contact Support
2. **Merchant Number (`merchantNo`)**
3. **Merchant API Key (`apiKey`)**

---

### Signature (`sign`) Algorithm
```
BCrypt(Base64(SHA-256(apiKey + originalStr + apiKey)))
```
1. **`originalStr`**: Concatenate all non-empty parameter values (**excluding the `sign` parameter**) in alphabetical order of their names (`name=UrlEncode(value)`), separated by `&`.
   - **Note:** Parameters with empty values (null or empty strings) are excluded from the signature.
   - **Note:** `value` must be URL Encoded.
   
2. **Algorithm Steps:**
   1. Use the SHA-256 algorithm to hash **`apiKey + originalStr + apiKey`**, producing a binary signature.
   2. Encode the SHA-256 binary signature using Base64.
   3. Generate the final signature string using BCrypt on the Base64-encoded string.

3. **Official SDK Examples:**
   - [PHP Demo](https://github.com/rongpay/rongpay.github.io/tree/master/php-demo)
   - [Java Demo](https://github.com/rongpay/rongpay.github.io/tree/master/java-demo)
   - [C# Demo](https://github.com/rongpay/rongpay.github.io/tree/master/C%23-demo)

---

### Synchronous Notifications (`returnUrl`)
When the order creation request includes a return URL, users will be redirected to this URL upon order completion. The return URL will carry parameters (`returnUrl?urlparams`). Refer to [Unified Return Parameters](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0) for parameter details. Example:

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

---

### Asynchronous Callbacks (`notifyUrl`)
If an asynchronous callback URL is provided during order creation, the system will send a notification after order completion (status **50**). Notifications will be sent a total of 3 times at intervals of 0s, 15s, and 60s. Each notification times out after 10 seconds. 

- Respond with **`success`** to indicate successful processing.
- Responding with any other value will prompt the system to retry notification.

Parameters are provided as per [Unified Return Parameters](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0). Example:

```
curl -X POST "Callback URL"
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

---

Here's the professional English translation for the remaining segments: 

---

### 1. Order API
#### 1.1 Create Order API
1. **Use Case**: When a user initiates a top-up/recharge, the merchant generates a redirection URL based on the parameters below. The user can then be redirected to the payment page to complete the payment.
2. **Request Type**: Page Redirection
3. **Endpoint**: `Gateway Address + /pay-order/#/?urlparams`
4. **Request Parameters**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `amount`       | Yes      | Integer   | 100                            | Amount in cents. Minimum value: 100 (1 Legal Tender). |
| `merchantNo`   | Yes      | String    | 20191204192421307122140114     | Merchant Number                          |
| `name`         | Yes      | String    | Zhang San                     | Payer’s Name                             |
| `orderNo`      | Yes      | String (<50) | 201912081855183951ab02e       | Merchant Order Number                    |
| `payMode`      | Yes      | String    | 100001                         | Payment Mode (Available in the merchant dashboard) |
| `ts`           | Yes      | Integer   | 1575948756                     | Merchant Order Timestamp (in seconds)   |
| `notifyUrl`    | No       | String    | https://www.baidu.com/notify   | Backend Notification URL                |
| `returnUrl`    | No       | String    | https://www.baidu.com          | User redirection URL post-payment       |
| `sign`         | Yes      | String    | $2a$10$JwOX9nmVHrE6o8vcoSmyd.T6... | Signature generated using the specified signing algorithm |

6. **Response**:  
5. **Example**:  
```
Gateway Address + /pay-order/#/?amount=100&merchantNo=20191204192421307122140114&orderNo=1575730270288&payMode=100001&ts=1575730270&sign=%242a%2410%24JwOX9nmVHrE6o8vcoSmyd.T69Yl7n322tVLmz.pVkRUz%2F.tRCjELS
```

---

#### 1.2 Query Order API
1. **Use Case**: When the merchant needs to query the details of a specific order.
2. **Request Type**: POST
3. **Endpoint**: `Gateway Address + /any-pay/open/order/query`
4. **Request Parameters**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `merchantNo`   | Yes      | String    | 20191204192421307122140114     | Merchant Number                          |
| `orderNo`      | Yes      | String (<50) | 201912081855183951ab02e       | Merchant Order Number                    |
| `ts`           | Yes      | Integer   | 1575948756                     | Merchant Order Timestamp (in seconds)   |
| `sign`         | Yes      | String    | $2a$10$JwOX9nmVHrE6o8vcoSmyd.T6... | Signature generated using the specified signing algorithm |

6. **Response**: Refer to [Unified Return Parameters](https://rongpay.github.io/#%E7%BB%9F%E4%B8%80%E8%BF%94%E5%9B%9E%E5%8F%82%E6%95%B0).  
5. **Example**:  
```
Request:
curl -X POST "Gateway Address + /any-pay/open/order/query" \
-H "accept:*/*" \
-H "Content-Type:application/json" \
-d "{\"merchantNo\":\"20191204192421307122140114\",\"orderNo\":\"201912081855183951ab02e\",\"sign\":\"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T69Yl7n322tVLmz.pVkRUz/.tRCjELS\",\"ts\":1575948756}"
Response:
```

---

### Unified Return Parameters
1. **Parameter Details**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `amount`       | Yes      | Integer   | 100                            | Amount in cents                          |
| `merchantNo`   | Yes      | String    | 20191204192421307122140114     | Merchant Number                          |
| `orderNo`      | Yes      | String (<50) | 201912081855183951ab02e       | Merchant Order Number                    |
| `payMode`      | Yes      | String    | 100001                         | Payment Mode                             |
| `ts`           | Yes      | Integer   | 1575948756                     | Merchant Order Timestamp (in seconds)   |
| `orderStatus`  | Yes      | Integer   | 50                             | Order Status (Refer to Order Status Enum below) |
| `payNo`        | No       | String    | 20191209194326631108714792     | Payment Order Number                     |
| `payStatus`    | No       | Integer   | 30                             | Payment Status (Refer to Payment Status Enum below) |
| `payTime`      | No       | Integer   | 1575948756                     | Payment Success Time (in seconds)       |
| `sign`         | Yes      | String    | $2a$10$JwOX9nmVHrE6o8vcoSmyd.T6... | Signature for validation (Use BCrypt for verification) |

---

2. **Order Status Enum (`orderStatus`)**:

| Value | Description                     |
|-------|---------------------------------|
| -20   | No available channels (No payment status will be provided) |
| 30    | Payment pending                 |
| -30   | User canceled order             |
| -40   | Payment timeout                 |
| -50   | Order failed                    |
| 50    | Order completed successfully    |

---

3. **Payment Status Enum (`payStatus`)**:

| Value | Description                     |
|-------|---------------------------------|
| 10    | Awaiting payment                |
| -10   | Payment timeout                 |
| -20   | Payment canceled                |
| 30    | Payment successful              |
| -30   | Payment failed                  |

---

**Important Note:** The primary determinant is the `orderStatus`. In cases of payment timeout, a "payment successful" notification may still be received. Make sure to handle such cases appropriately.

---

### 2. Payout API
#### 2.1 Payout Application API
1. **Use Case**: Used to initiate a payout.
2. **Request Type**: POST
3. **Endpoint**: `Gateway Address + /any-pay/open/merchant/transfer/apply`
4. **Required Request Header**: 
   - `X-REQUEST-TOKEN`: Random string
5. **Request Parameters**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `merchantNo`   | Yes      | String    | 20191204192421307122140114     | Merchant Number                          |
| `amount`       | Yes      | Integer   | 500000                         | Amount in cents                          |
| `orderNo`      | Yes      | String    | 1735965033012                  | Order Number                             |
| `payMode`      | Yes      | String    | x2001                          | Mode identifier (Contact support)        |
| `account`      | Yes      | String    | xxxxx                          | Recipient account                        |
| `name`         | Yes      | String    | Zhang San                     | Recipient name                           |
| `notifyUrl`    | Yes      | String    | http://www.baidu.com           | Callback notification URL                |
| `ts`           | Yes      | Integer   | 1575948756                     | Timestamp in seconds                     |
| `sign`         | Yes      | String    | $2a$10$JwOX9nmVHrE6o8vcoSmyd.T6... | Request signature                        |

---

#### 2.2 Payout Query API
1. **Use Case**: Used to query the details of a specific payout request.
2. **Request Type**: POST
3. **Endpoint**: `Gateway Address + /any-pay/open/merchant/transfer/query`
4. **Required Request Header**: 
   - `X-REQUEST-TOKEN`: Random string
5. **Request Parameters**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `merchantNo`   | Yes      | String    | 20191204192421307122140114     | Merchant Number                          |
| `orderNo`      | Yes      | String    | 1735965033012                  | Order Number                             |
| `ts`           | Yes      | Integer   | 1575948756                     | Timestamp in seconds                     |
| `sign`         | Yes      | String    | $2a$10$JwOX9nmVHrE6o8vcoSmyd.T6... | Request signature                        |

6. **Response**: Refer to [Payout Response Content](https://rongpay.github.io/#代付响应内容).  
5. **Example**:  
```
Request:  
curl --location 'Gateway Address + /any-pay/open/merchant/transfer/query' \
--header 'Content-Type: application/json' \
--data '{
    "merchantNo":"20241218110252173100554114",
    "ts":1735897292,
    "sign":"$2a$10$WU1OaPTcyP/zfSLPRUW8eeLXQNO824VpDGYwCKGJoktUoLaICA8Z.",
    "orderNo":"1735894585397"
}'
Response:
{
    "merchantNo":"20241218110252173100554114",
    "orderNo":"1735965033012",
    "applyNo":"20250104123046200142181117",
    "amount":1000,
    "serviceCharge":101,
    "status":13
}
```

---

### Payout Response Content
1. **Response Parameters**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `merchantNo`   | Yes      | String    | 20241218110252173100554114     | Merchant Number                          |
| `orderNo`      | Yes      | String    | 1735965033012                  | Order Number                             |
| `applyNo`      | Yes      | String    | 201912041924213071221490224    | Application Number                       |
| `amount`       | Yes      | Integer   | 500000                         | Amount in cents                          |
| `serviceCharge`| Yes      | Integer   | 300                            | Service fee amount in cents              |
| `status`       | Yes      | Integer   | 10                             | Application status                       |
| `ts`           | Yes      | Integer   | 1575948756                     | Timestamp (in seconds), included in callback notifications |
| `sign`         | Yes      | String    | $2a$10$JwOX9nmVHrE6o8vcoSmyd.T6... | Signature generated using the specified signing algorithm, included in callback notifications |

---

2. **Application Status Enum (`status`)**:

| Value | Description                     |
|-------|---------------------------------|
| 10    | Awaiting processing             |
| 13    | Payment in progress             |
| 16    | Pending confirmation            |
| 20    | Successful                      |
| -20   | Failed                          |

---

3. **Example**:
```
{
    "merchantNo":"20241218110252173100554114",
    "orderNo":"1735965033012",
    "applyNo":"20250104123046200142181117",
    "amount":1000,
    "serviceCharge":101,
    "status":13
}
```

---

#### 2.3 Payout Asynchronous Callback (`notifyUrl`)
When an asynchronous callback URL (`notifyUrl`) is provided during the payout application, the system will notify you after the payout process finishes (either successful `20` or failed `-20`). The system will send the notification a total of 3 times at intervals of 0s, 15s, and 60s. Each notification will time out after 10 seconds.

- Respond with **`success`** to indicate successful processing.
- Responding with any other value will result in the system retrying the notification.

Parameters mirror those outlined in [Unified Response Parameters](https://rongpay.github.io/#代付响应内容). Example:

```
curl -X POST "Callback Address"
  -H 'content-type: application/json' 
  -d '{
    "merchantNo":"20241218110252173100554114",
    "orderNo":"1735965033012",
    "applyNo":"20250104123046200142181117",
    "amount":1000,
    "serviceCharge":101,
    "status":-20,
    "ts":1735965414,
    "sign":"$2a$10$xaxWqSrekcFwniMfHr460ueXW5LfkmtqkBYqOxGLntu8Bp5pgmLQe"
}'
```

---

### 3. Disbursement API
#### 3.1 Bank Card Disbursement API
1. **Use Case**: For merchants to initiate a disbursement to a bank account.
2. **Request Type**: POST
3. **Endpoint**: `Gateway Address + /any-pay/open/merchant/withdraw-apply`
4. **Required Request Header**: 
   - `X-REQUEST-TOKEN`: Random string
5. **Request Parameters**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `merchantNo`   | Yes      | String    | 20191204192421307122140114     | Merchant Number                          |
| `bankName`     | Yes      | String    | Bank of China                  | Name of the bank                         |
| `bankcard`     | Yes      | String    | 6225804598346543               | Bank card number                         |
| `realName`     | Yes      | String    | Zhang San                     | Cardholder's name                        |
| `passwd`       | Yes      | String    | MD5(Password)                  | Withdrawal password (MD5 encrypted)     |
| `amt`          | Yes      | Integer   | 500000                         | Amount (in cents, range: 500000 to 5000000) |
| `ts`           | Yes      | Integer   | 1575948756                     | Timestamp (in seconds)                   |
| `sign`         | Yes      | String    | $2a$10$JwOX9nmVHrE6o8vcoSmyd.T6... | Signature generated using the specified signing algorithm |

---

6. **Response**: Refer to [Disbursement Response Content](https://rongpay.github.io/#%E5%93%8D%E5%BA%94%E5%86%85%E5%AE%B9).  
5. **Example**:  
```
Request:  
curl -X POST "Gateway + /any-pay/open/merchant/withdraw-apply" \
-H "X-REQUEST-TOKEN:111111we2324" \
-H "Content-Type:application/json" \
-d '{
    "amt":500000,
    "bankName":"Bank of China",
    "bankcard":"6225804598346543",
    "merchantNo":"20191204192421307122140114",
    "passwd":"MD5(Password)",
    "realName":"Zhang San",
    "sign":"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6",
    "ts":1575948756
}'
```

---

#### 3.2 Disbursement Query API
1. **Use Case**: To query the status of a specific disbursement application.
2. **Request Type**: POST
3. **Endpoint**: `Gateway Address + /any-pay/open/merchant/withdraw-apply/query`
4. **Required Request Header**: 
   - `X-REQUEST-TOKEN`: Random string
5. **Request Parameters**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `merchantNo`   | Yes      | String    | 20191204192421307122140114     | Merchant Number                          |
| `applyNo`      | Yes      | String    | 201912041924213071221490224    | Application Number                       |
| `ts`           | Yes      | Integer   | 1575948756                     | Timestamp (in seconds)                   |
| `sign`         | Yes      | String    | $2a$10$JwOX9nmVHrE6o8vcoSmyd.T6... | Signature generated using the specified signing algorithm |

---

6. **Response**: Refer to [Disbursement Response Content](https://rongpay.github.io/#%E5%93%8D%E5%BA%94%E5%86%85%E5%AE%B9).  
5. **Example**:  
```
Request:  
curl -X POST "Gateway + /any-pay/open/merchant/withdraw-apply/query" \
-H "X-REQUEST-TOKEN:111111we2324" \
-H "Content-Type:application/json" \
-d '{
    "merchantNo":"20191204192421307122140114",
    "applyNo":"201912041924213071221490224",
    "sign":"$2a$10$JwOX9nmVHrE6o8vcoSmyd.T6",
    "ts":1575948756
}'
```

---

### Disbursement Response Content
1. **Response Parameters**:

| Parameter Name | Required | Data Type | Example                        | Description                              |
|----------------|----------|-----------|--------------------------------|------------------------------------------|
| `type`         | Yes      | String    | WITHDRAW                       | Type of transaction                      |
| `applyNo`      | Yes      | String    | 201912041924213071221490224    | Application Number                       |
| `amt`          | Yes      | Integer   | 500000                         | Amount in cents                          |
| `serviceCharge`| Yes      | Integer   | 300                            | Service fee in cents                     |
| `applyStatus`  | Yes      | Integer   | 10                             | Application status                       |

---

2. **Application Status Enum (`applyStatus`)**:

| Value | Description                     |
|-------|---------------------------------|
| 10    | Awaiting processing             |
| 13    | Payment in progress             |
| 16    | Pending confirmation            |
| 20    | Successful                      |
| -20   | Failed                          |

---

3. **Example**:
```
{
    "applyNo":"20200323111021811157255464",
    "type":"WITHDRAW",
    "amt":500000,
    "serviceCharge":300,
    "applyStatus":20
}
```
