﻿window.authManager = {
    saveToken: function (token) {
        sessionStorage.setItem('authToken', token);
    },
    getToken: function () {
        return sessionStorage.getItem('authToken');
    },
    removeToken: function () {
        sessionStorage.removeItem('authToken');
    }
};