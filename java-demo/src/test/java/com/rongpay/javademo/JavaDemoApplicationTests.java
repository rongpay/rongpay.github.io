package com.rongpay.javademo;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.rongpay.javademo.commom.ApiUtil;
import com.rongpay.javademo.commom.AppProperties;
import com.rongpay.javademo.service.ApiService;
import com.rongpay.javademo.service.RequestService;
import lombok.extern.slf4j.Slf4j;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;

import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultHandlers.print;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
@AutoConfigureMockMvc
@Slf4j
class JavaDemoApplicationTests {

    @Autowired
    private ApiService apiService;
    @Autowired
    private AppProperties appProperties;
    @Autowired
    private RequestService requestService;
    @Autowired
    private ObjectMapper objectMapper;
    @Autowired
    private MockMvc mockMvc;

    /**
     * 测试创建订单
     */
    @Test
    void testGenerateCreateOrder() {
        Map<String, String> param = new HashMap<>();
        param.put("amount", "100");
        param.put("merchantNo", appProperties.getMerchantNo());
        param.put("name", "张三");
        param.put("orderNo", UUID.randomUUID().toString().replace("-", ""));
        param.put("payMode", "pay_mode.4");
        param.put("ts", System.currentTimeMillis() / 1000 + "");
        param.put("notifyUrl", "https://www.yourdomain.com/callback/notify-callback");
        param.put("returnUrl", "https://www.yourdomain.com/callback/return-callback");
        log.info(apiService.generateCreateOrderLink(param));
    }

    /**
     * 测试查询订单
     */
    @Test
    void testQueryOrder() {
       queryOrder();
    }

    Map<String, String> queryOrder() {
        Map<String, String> param = new HashMap<>();
        param.put("merchantNo", appProperties.getMerchantNo());
        param.put("orderNo", "1576061857");
        param.put("ts", System.currentTimeMillis() / 1000 + "");
        param.put("sign", ApiUtil.generateSign(param, appProperties.getApiKey()));
        Map<String, String> queryOrder = requestService.queryOrder(param);
        log.info(queryOrder.toString());
        return queryOrder;
    }

    /**
     * 测试返回回调
     */
    @Test
    void testReturnCallback() throws Exception {
        Map<String, String> queryOrder = queryOrder();
        MultiValueMap<String, String> multiValueMap=new LinkedMultiValueMap<>();
        queryOrder.forEach(multiValueMap::add);
        mockMvc.perform(get("/callback/return-callback")
                .contentType(MediaType.APPLICATION_JSON).params(multiValueMap))
                .andExpect(status().isOk())
                .andDo(print())
                .andReturn().getResponse().getContentAsString();
    }

    /**
     * 测试通知回调
     */
    @Test
    void testNotifyCallback() throws Exception {
        Map<String, String> queryOrder = queryOrder();
        mockMvc.perform(post("/callback/notify-callback")
                .contentType(MediaType.APPLICATION_JSON).content(objectMapper.writeValueAsString(queryOrder)))
                .andExpect(status().isOk())
                .andDo(print())
                .andReturn().getResponse().getContentAsString();
    }
}
