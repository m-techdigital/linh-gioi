package com.linhgioi.server.api.auth;

public record ProductRegisterRequest(String email, String password, boolean acceptedTerms) { }
