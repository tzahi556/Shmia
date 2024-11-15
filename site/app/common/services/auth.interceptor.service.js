(function () {
	var app = angular.module('app');

	app.service('authInterceptorService', AuthInterceptorService);

	function AuthInterceptorService($q, $location) {

		this.request = _request;
		this.responseError = _responseError;
		

		function _request(config) {

			config.headers = config.headers || {};

			var authData = localStorage.getItem('authorizationData');
			if (authData) {
				authData = angular.fromJson(authData);
				config.headers.Authorization = 'Bearer ' + authData.token;
			}

			return config;
		}

		function _responseError(rejection) {

			

			if (rejection.status === 401) {

				
				//login({ userName: "tzahi556@gmail.com", password: "123" });

				//authenticationService.login({ userName: "tzahi556@gmail.com", password: "123" }).then(function (res) {

				//	location.href = './';
				//},
				//	function (res) {
				//		alert(res.error_description);
				//	});
				//http://localhost:61957/#/worker/1880/

				$location.path('/login/');
				//$location.path('/login/');
			}

			return $q.reject(rejection);
		}

		
	}

})();