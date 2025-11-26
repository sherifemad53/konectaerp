package com.apigateway.apigate;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.client.discovery.EnableDiscoveryClient;

@SpringBootApplication
@EnableDiscoveryClient
public class ApigateApplication {

	private static final org.slf4j.Logger logger = org.slf4j.LoggerFactory.getLogger(ApigateApplication.class);

	public static void main(String[] args) {
		SpringApplication.run(ApigateApplication.class, args);
		logger.info("API Gateway Application Main Method Executed");
	}

	@org.springframework.context.annotation.Bean
	public org.springframework.boot.CommandLineRunner commandLineRunner(org.springframework.core.env.Environment env) {
		return args -> {
			logger.info("==================================================================================");
			logger.info("API Gateway Application Started");
			logger.info("Server Port: {}", env.getProperty("server.port"));
			logger.info("Management Port: {}", env.getProperty("management.server.port"));
			logger.info("Consul Host: {}", env.getProperty("spring.cloud.consul.host"));
			logger.info("Consul Port: {}", env.getProperty("spring.cloud.consul.port"));

			logger.info("AUTHENTICATION SERVICE URL: {}", env.getProperty("upstreams.authentication"));
			logger.info("HR SERVICE URL: {}", env.getProperty("upstreams.hr"));
			logger.info("USER MANAGEMENT SERVICE URL: {}", env.getProperty("upstreams.userManagement"));
			logger.info("INVENTORY SERVICE URL: {}", env.getProperty("upstreams.inventory"));
			logger.info("FINANCE SERVICE URL: {}", env.getProperty("upstreams.finance"));
			logger.info("REPORTING SERVICE URL: {}", env.getProperty("upstreams.reporting"));
			logger.info("==================================================================================");


			
		};
	}

}
