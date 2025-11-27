package com.apigateway.apigate.controller;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import reactor.core.publisher.Mono;

import java.time.OffsetDateTime;
import java.util.Map;

@RestController
@RequestMapping("/debug")
public class DebugHealthController {

    private static final Logger logger = LoggerFactory.getLogger(DebugHealthController.class);

    @GetMapping("/health")
    public Mono<ResponseEntity<Map<String, Object>>> debugHealth() {
        logger.info("Debug health check endpoint called");
        Map<String, Object> status = Map.of(
                "status", "UP",
                "timestamp", OffsetDateTime.now().toString(),
                "message", "API Gateway is running and reachable"
        );
        return Mono.just(ResponseEntity.ok(status));
    }
}
