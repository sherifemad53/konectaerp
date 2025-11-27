package com.apigateway.apigate.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;
import reactor.core.publisher.Mono;

@RestController
public class RootController {

    @GetMapping("/")
    public Mono<ResponseEntity<String>> root() {
        return Mono.just(ResponseEntity.ok("API Gateway is running"));
    }
}
