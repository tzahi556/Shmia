﻿(function () {

    var app = angular.module('app', ['ui.router', 'angular-loading-bar', 'ui.bootstrap']);

    app.config(function ($urlRouterProvider, $stateProvider) {
       
        $urlRouterProvider.otherwise(function ($inject) {
            $state = $inject.get('$state');
            usersService = $inject.get('usersService');
            var roles = usersService.roles;
            var role = localStorage.getItem('currentRole');


            if (role == null) {

                $state.go('login');
            }
            for (var i in roles) {
                if (roles[i].id == role) {

                    $state.go(roles[i].homePage);
                }
            }
        });

        $stateProvider.state('login', {
            url: '/login/{returnUrl}',
            views: {
                'main': {
                    template: '<login return-url="$ctrl.returnUrl"></login>'
                },
                controller: function ($stateParams) {

                    this.returnUrl = $stateParams.returnUrl;

                }
            }
        });

        $stateProvider.state('docs', {
            url: '/docs/',
            views: {
                'main': {
                    template: '<docs></docs>'
                }
            }
        });


        $stateProvider.state('reportarea', {
            url: '/reportarea/',
            views: {
                'main': {
                    template: '<reportarea reportarea="$ctrl.reportarea"></reportarea>',
                    controller: function (reportarea) {
                        this.reportarea = reportarea;
                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        reportarea: function (usersService) {
                            return usersService.getReportData(1);
                        }
                    }
                }
            }
        });





        $stateProvider.state('workers', {
            url: '/workers/',
            views: {
                'main': {
                    template: '<workers workers="$ctrl.workers"></workers>',
                    controller: function (workers) {

                        this.workers = workers;


                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        workers: function (usersService) {
                            return usersService.getWorkers(true);
                        }
                    }
                }
            }
        });

        $stateProvider.state('workersnew', {
            url: '/workersnew/',
            views: {
                'main': {
                    template: '<workersnew workersnew="$ctrl.workersnew"></workersnew>',
                    controller: function (workersnew) {

                        this.workersnew = workersnew;


                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        workersnew: function (usersService) {

                            return usersService.getWorkers(false);
                        }
                    }
                }
            }
        });

        $stateProvider.state('worker', {
            url: '/worker/{id}/',
            views: {
                'main': {
                    template: '<worker farm="$ctrl.farm" screendata="$ctrl.screendata" users="$ctrl.users" worker="$ctrl.worker" files="$ctrl.files" childs="$ctrl.childs" cities="$ctrl.cities" banks="$ctrl.banks"  banksbrunchs="$ctrl.banksbrunchs" ></worker>',
                    controller: function (worker, files, childs, cities, banks, banksbrunchs, users, screendata,farm) {
                        this.worker = worker;
                        this.files = files;
                        this.childs = childs;
                        this.cities = cities;
                        this.banks = banks;
                        this.banksbrunchs = banksbrunchs;
                        this.users = users;
                        this.screendata = screendata;
                        this.farm = farm;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        worker: function (usersService, $stateParams) {

                            return usersService.getWorker($stateParams.id);
                        },
                        files: function (usersService, $stateParams) {

                            return usersService.getFiles($stateParams.id);
                        },
                        childs: function (usersService, $stateParams) {

                            return usersService.getWorkerChilds($stateParams.id);
                        },

                        cities: function (usersService, $stateParams) {

                            return usersService.getMasterTable(1);
                        },
                        banks: function (usersService, $stateParams) {

                            return usersService.getMasterTable(2);
                        },
                        banksbrunchs: function (usersService, $stateParams) {

                            return usersService.getMasterTable(3);
                        },

                        users: function (usersService) {
                            if (localStorage.getItem('currentRole')=="farmAdmin")
                                return usersService.getUsers("instructor");
                        },

                        screendata: function (farmsService, $stateParams) {
                           
                            return farmsService.getSetWorkerAndCompanyData(1,$stateParams.id,null);
                        },

                        farm: function (farmsService, $stateParams) {

                            return farmsService.getSetWorkerAndCompanyData(3,$stateParams.id, null);
                        }

                    }
                }
            }
        });

        $stateProvider.state('workernew', {
            url: '/workernew/{id}/',
            views: {
                'main': {
                    template: '<workernew workernew="$ctrl.workernew" files="$ctrl.files" childs="$ctrl.childs" cities="$ctrl.cities" banks="$ctrl.banks"  banksbrunchs="$ctrl.banksbrunchs" ></workernew>',
                    controller: function (workernew, files, childs, cities, banks, banksbrunchs) {
                        this.workernew = workernew;
                        this.files = files;
                        this.childs = childs;
                        this.cities = cities;
                        this.banks = banks;
                        this.banksbrunchs = banksbrunchs;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        workernew: function (usersService, $stateParams) {

                            return usersService.getWorker($stateParams.id);
                        },
                        files: function (usersService, $stateParams) {

                            return usersService.getFiles($stateParams.id);
                        },
                        childs: function (usersService, $stateParams) {

                            return usersService.getWorkerChilds($stateParams.id);
                        },

                        cities: function (usersService, $stateParams) {

                            return usersService.getMasterTable(1);
                        },
                        banks: function (usersService, $stateParams) {

                            return usersService.getMasterTable(2);
                        },
                        banksbrunchs: function (usersService, $stateParams) {

                            return usersService.getMasterTable(3);
                        }


                    }
                }
            }
        });

        $stateProvider.state('users', {
            url: '/users/',
            views: {
                'main': {
                    template: '<users users="$ctrl.users"></users>',
                    controller: function (users) {

                        this.users = users;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        users: function (usersService) {

                            return usersService.getUsers();
                        }
                    }
                }
            }
        });

        $stateProvider.state('user', {
            url: '/user/{id}/',
            views: {
                'main': {
                    template: '<user user="$ctrl.user" ></user>',
                    controller: function (user) {
                        this.user = user;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        user: function (usersService, $stateParams) {
                            return usersService.getUser($stateParams.id);
                        }
                    }
                }
            }
        });

        $stateProvider.state('logs', {
            url: '/logs/',
            views: {
                'main': {
                    template: '<logs users="$ctrl.users"></logs>',
                    controller: function (users) {
                        this.users = users;
                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        users: function (usersService) {
                            return usersService.getUsers(['instructor', 'profAdmin']);
                        }
                    }
                }
            }
        });

        $stateProvider.state('farms', {
            url: '/farms/',
            views: {
                'main': {
                    template: '<farms farms="$ctrl.farms"></farms>',
                    controller: function (farms) {
                        this.farms = farms;
                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        farms: function (farmsService) {
                            
                            return farmsService.getFarms();
                        }
                    }
                }
            }
        });

        $stateProvider.state('farm', {
            url: '/farm/{id}',
            views: {
                'main': {
                    template: '<farm farm="$ctrl.farm"></farm>',
                    controller: function (farm) {
                        this.farm = farm;
                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        farm: function (farmsService, $stateParams) {
                            return farmsService.getFarm($stateParams.id);
                        }
                    }
                }
            }
        });

        $stateProvider.state('awsmangershistory', {
            url: '/awsmangershistory/',
            views: {
                'main': {
                    template: '<awsmangershistory portfolios="$ctrl.portfolios" ></awsmangershistory>',
                    controller: function (portfolios) {

                        this.portfolios = portfolios;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        portfolios: function (usersService) {

                            return usersService.getPortfolios();
                        }

                    }
                }
            }
        });

        $stateProvider.state('awsmangershistoryphrase', {
            url: '/awsmangershistoryphrase/',
            views: {
                'main': {
                    template: '<awsmangershistoryphrase portfolios="$ctrl.portfolios" ></awsmangershistoryphrase>',
                    controller: function (portfolios) {

                        this.portfolios = portfolios;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        portfolios: function (usersService) {

                            return usersService.getPortfolios();
                        }

                    }
                }
            }
        });

        $stateProvider.state('awsmangers', {
            url: '/awsmangers/',
            views: {
                'main': {
                    template: '<awsmangers portfolios="$ctrl.portfolios" ></awsmangers>',
                    controller: function (portfolios) {
                        this.portfolios = portfolios;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        portfolios: function (usersService) {

                            return usersService.getPortfolios(100, 760, 500, 200, "מה קקקקק", 14, 1, -1, 5);
                        }

                    }
                }
            }
        });

        $stateProvider.state('bindtodata', {
            url: '/bindtodata/',
            views: {
                'main': {
                    template: '<bindtodata portfolios="$ctrl.portfolios" ></bindtodata>',
                    controller: function (portfolios) {
                        this.portfolios = portfolios;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        portfolios: function (usersService) {

                            return usersService.getPortfolios(100, 760, 500, 200, "מה קקקקק", 14, 1, -1, 1);
                        }

                    }
                }
            }
        });

        $stateProvider.state('awsauto', {
            url: '/awsauto/{id}/{name}',
            views: {
                'main': {
                    template: '<awsauto portfolioasins="$ctrl.portfolioasins" protname="$ctrl.protname" portfolio="$ctrl.portfolio" portfolioid="$ctrl.portfolioid"></awsauto>',
                    controller: function (portfolioasins, protname, portfolio, portfolioid) {
                        this.portfolioasins = portfolioasins;
                        this.protname = protname;
                        this.portfolio = portfolio;
                        this.portfolioid = portfolioid;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        portfolioasins: function (usersService, $stateParams) {
                            return usersService.getAllAsins($stateParams.id);
                        },

                        protname: function (usersService, $stateParams) {
                            return $stateParams.name;
                        },

                        portfolio: function (usersService) {
                            return usersService.getPortfolios();
                        },

                        portfolioid: function (usersService, $stateParams) {
                            return $stateParams.id;
                        },

                    }
                }
            }
        });

        $stateProvider.state('awspools', {
            url: '/awspools/{id}/{name}',
            views: {
                'main': {
                    template: '<awspools  portfolio="$ctrl.portfolio" portfoliowords="$ctrl.portfoliowords" protname="$ctrl.protname" portfolioid="$ctrl.portfolioid"></awspools>',
                    controller: function (portfoliowords, protname, portfolioid, portfolio) {
                        this.portfoliowords = portfoliowords;
                        this.protname = protname;
                        this.portfolioid = portfolioid;
                        this.portfolio = portfolio;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        portfoliowords: function (usersService, $stateParams) {
                            return usersService.getAllPortfliositemsForWords($stateParams.id, 2);
                        },

                        protname: function (usersService, $stateParams) {
                            return $stateParams.name;
                        },

                        portfolioid: function (usersService, $stateParams) {
                            return $stateParams.id;
                        },

                        portfolio: function (usersService) {
                            return usersService.getPortfolios();
                        },

                    }
                }
            }
        });

        $stateProvider.state('farmmanager', {
            url: '/farmmanager/',
            views: {
                'main': {
                    template: '<farmmanager farmmanager="$ctrl.farmmanager" btns="$ctrl.btns"  grps="$ctrl.grps"  btns2grps="$ctrl.btns2grps" farm="$ctrl.farm" farmspdffiles="$ctrl.farmspdffiles"></farmmanager>',
                    controller: function (farmmanager, farm, farmspdffiles, btns, grps, btns2grps) {
                        this.farm = farm;
                        this.farmmanager = farmmanager;
                        this.farmspdffiles = farmspdffiles;
                        this.btns = btns;
                        this.grps = grps;
                        this.btns2grps = btns2grps;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        farmmanager: function (farmsService, $stateParams) {
                            
                            return [];
                        },

                        farm: function (farmsService, $stateParams) {
                            
                            return farmsService.getFarm(localStorage.getItem('FarmId'));
                        },
                        farmspdffiles: function (farmsService) {

                            return farmsService.getFarmPDFFiles(localStorage.getItem('FarmId'),-1);
                        },


                        btns: function (farmsService) {

                            return farmsService.actionFieldGroup(1,localStorage.getItem('FarmId'),null);
                        },

                        grps: function (farmsService) {

                            return farmsService.actionFieldGroup(2,localStorage.getItem('FarmId'),null);
                        },
                        btns2grps: function (farmsService) {

                            return farmsService.actionFieldGroup(3,localStorage.getItem('FarmId'),null);
                        }

                        //,
                      
                        //horses: function (horsesService) {
                        //    debugger

                        //    return [];
                        //},
                        //horsegroups: function (horsesService) {
                           
                        //    return [];
                        //},

                        //horsegroupshorses: function (horsesService) {
                           
                        //    return [];
                        //},


                    }
                }
            }
        });

        $stateProvider.state('campains', {
            url: '/campains/',
            views: {
                'main': {
                    template: '<campains campains="$ctrl.campains"></campains>',
                    controller: function (campains) {

                        this.campains = campains;


                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        campains: function (farmsService) {
                            return farmsService.getSetCampainsData(1, localStorage.getItem('FarmId'),null);
                        }
                    }
                }
            }
        });

        $stateProvider.state('campain', {
            url: '/campain/{id}/',
            views: {
                'main': {
                    template: '<campain workers="$ctrl.workers" campain="$ctrl.campain" btns="$ctrl.btns"  grps="$ctrl.grps"  btns2grps="$ctrl.btns2grps" farm="$ctrl.farm" farmspdffiles="$ctrl.farmspdffiles"></campain>',
                    controller: function (campain, farm, farmspdffiles, btns, grps, btns2grps, workers) {
                        this.farm = farm;
                        this.campain = campain;
                        this.farmspdffiles = farmspdffiles;
                        this.btns = btns;
                        this.grps = grps;
                        this.btns2grps = btns2grps;
                        this.workers = workers;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        campain: function (farmsService, $stateParams) {
                            return farmsService.getSetCampainsData(2, $stateParams.id, null);
                        },

                        farm: function (farmsService, $stateParams) {

                            return farmsService.getFarm(localStorage.getItem('FarmId'));
                        },

                        farmspdffiles: function (farmsService, $stateParams) {

                            return farmsService.getFarmPDFFiles(localStorage.getItem('FarmId'), $stateParams.id);
                        },

                        btns: function (farmsService, $stateParams) {

                            return farmsService.actionFieldGroup(1, localStorage.getItem('FarmId'), null, $stateParams.id);
                        },

                        grps: function (farmsService, $stateParams) {

                            return farmsService.actionFieldGroup(2, localStorage.getItem('FarmId'), null, $stateParams.id);
                        },

                        btns2grps: function (farmsService, $stateParams) {

                            return farmsService.actionFieldGroup(3, localStorage.getItem('FarmId'), null, $stateParams.id);
                        },

                        workers: function (farmsService, $stateParams) {
                             return farmsService.getSetCampainsData(4, $stateParams.id, null);
                        }


                    }
                }
            }
        });

        $stateProvider.state('workercampains', {
            url: '/workercampains/{id}/{campainid}/',
            views: {
                'main': {
                    template: '<workercampains  screendata="$ctrl.screendata" worker="$ctrl.worker" campain="$ctrl.campain" ></workercampains>',
                    controller: function (screendata, worker, campain) {
                        this.worker = worker;
                        this.campain = campain;
                        //this.childs = childs;
                        //this.cities = cities;
                        //this.banks = banks;
                        //this.banksbrunchs = banksbrunchs;
                        //this.users = users;
                        this.screendata = screendata;
                        //this.farm = farm;

                    },
                    controllerAs: '$ctrl',
                    resolve: {
                        worker: function (farmsService, $stateParams) {

                            return farmsService.getSetCampainsData(6, $stateParams.id, null);
                        },
                        campain: function (farmsService, $stateParams) {

                            return farmsService.getSetCampainsData(5, $stateParams.campainid, null);
                        },
                        //childs: function (usersService, $stateParams) {

                        //    return usersService.getWorkerChilds($stateParams.id);
                        //},

                        //cities: function (usersService, $stateParams) {

                        //    return usersService.getMasterTable(1);
                        //},
                        //banks: function (usersService, $stateParams) {

                        //    return usersService.getMasterTable(2);
                        //},
                        //banksbrunchs: function (usersService, $stateParams) {

                        //    return usersService.getMasterTable(3);
                        //},

                        //users: function (usersService) {
                        //    if (localStorage.getItem('currentRole') == "farmAdmin")
                        //        return usersService.getUsers("instructor");
                        //},

                        screendata: function (farmsService, $stateParams) {

                           

                            return farmsService.getSetWorkerAndCompanyData(1, $stateParams.id, null, $stateParams.campainid);
                        }

                        //farm: function (farmsService, $stateParams) {

                        //    return farmsService.getSetWorkerAndCompanyData(3, $stateParams.id, null);
                        //}

                    }
                }
            }
        });


        //$stateProvider.state('student', {
        //    url: '/student/{id}/',
        //    views: {
        //        'main': {
        //            template: '<student user="$ctrl.user" lessons="$ctrl.lessons" instructors="$ctrl.instructors" farms="$ctrl.farms" horses="$ctrl.horses"' 
        //                + 'horses="$ctrl.horses" payments="$ctrl.payments" files="$ctrl.files" commitments="$ctrl.commitments" expenses="$ctrl.expenses" userhorses="$ctrl.userhorses" students="$ctrl.students" makav="$ctrl.makav" ></student>',
        //            controller: function (user, lessons, instructors, farms, horses, payments, files, commitments, expenses, userhorses, students, makav) {
        //                this.user = user;
        //                this.lessons = lessons;
        //                this.instructors = instructors;
        //                this.farms = farms;
        //                this.horses = horses;
        //                this.payments = payments;
        //                this.files = files;
        //                this.commitments = commitments;
        //                this.expenses = expenses;
        //                this.userhorses = userhorses;
        //                this.students = students;

        //                this.makav = makav;

        //            },
        //            controllerAs: '$ctrl',
        //            resolve: {
        //                user: function (usersService, $stateParams) {

        //                   return usersService.getUser($stateParams.id,true);
        //                },
        //                lessons: function (lessonsService, $stateParams) {
        //                    return lessonsService.getLessons($stateParams.id);
        //                },
        //                instructors: function (usersService) {
        //                    return usersService.getUsers(['instructor', 'profAdmin'], true);
        //                },
        //                farms: function (farmsService) {
        //                    return farmsService.getFarms();
        //                },
        //                horses: function (horsesService) {
        //                    return horsesService.getHorses();
        //                },
        //                //*********************
        //                payments: function (usersService, $stateParams) {

        //                    return usersService.getPaymentsByUserId($stateParams.id);
        //                },
        //                files: function (usersService, $stateParams) {
        //                    return usersService.getUserFilesByUserId($stateParams.id);
        //                },
        //                commitments: function (usersService, $stateParams) {
        //                    return usersService.getUserCommitmentsByUserId($stateParams.id);
        //                },
        //                expenses: function (usersService, $stateParams) {
        //                    return usersService.getUserExpensesByUserId($stateParams.id);
        //                },
        //                userhorses: function (usersService, $stateParams) {
        //                    return usersService.getUserUserhorsesByUserId($stateParams.id);
        //                },

        //                students: function (usersService) {
        //                     return usersService.getUsers('student');
        //                },

        //                makav: function (usersService, $stateParams) {

        //                    return usersService.getUserUserMakavByUserId($stateParams.id);
        //                },



        //            }
        //        }
        //    }
        //});

    });


    app.filter('orderByDateDesc', function () {

        return function (items) {
            if (items) {

                items.sort(function (a, b) {

                    if (new Date(a.Date) > new Date(b.Date)) {
                        return -1;
                    }
                    else if (new Date(a.Date) < new Date(b.Date)) {
                        return 1;
                    }

                    else {

                        return 0;
                    }
                });


                return items;
            }
        }
    });

    app.config(function ($httpProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
    });

    app.run(function ($rootScope, $http, sharedValues) {

        var DeviceEnter = "";
        var ua = navigator.userAgent;
        if (/(tablet|ipad|playbook|silk)|(android(?!.*mobi))/i.test(ua)) {
            DeviceEnter = "tablet";
        }
        else if (/Mobile|Android|iP(hone|od)|IEMobile|BlackBerry|Kindle|Silk-Accelerated|(hpw|web)OS|Opera M(obi|ini)/.test(ua)) {
            DeviceEnter = "mobile";
        } else {
            DeviceEnter = "desktop";
        }

        $http.post(sharedValues.apiUrl + 'users/setUserDevice/', { DeviceEnter: DeviceEnter, UserAgent: ua }).then(function (res) {

            //var worker = res.data;
            //deferred.resolve(worker);
        });


        //document.addEventListener("dragstart", onOnline, false);
        //document.addEventListener("drop", onOffline, false);

        //function onOnline(event) {

        //    $rootScope.$apply(function () {
        //        $rootScope.noNetwork = event;
        //        $rootScope.studentIdDrag = event.target.id;
        //    });
        //}


        //function onOffline(event) {

        //    $rootScope.$apply(function () {
        //        $rootScope.noNetwork = event;
        //    });
        //}

        $rootScope.role = localStorage.getItem('currentRole');
        $rootScope.FarmInstractorPolicy = localStorage.getItem('FarmInstractorPolicy');



        $rootScope.IsInstructorBlock = ($rootScope.role == "instructor" && $rootScope.FarmInstractorPolicy == "true") ? true : false;

        $rootScope.isPhone = false;

        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            $rootScope.isPhone = true;
        }

        delete $http.defaults.headers.common['X-Requested-With'];

        $http.defaults.headers.common['Access-Control-Allow-Headers'] = 'origin, content-type, accept';
        $http.defaults.headers.common['Access-Control-Allow-Origin'] = '*';
        $http.defaults.headers.common['Access-Control-Allow-Methods'] = 'GET,POST,PUT,HEAD,DELETE,OPTIONS';

        var iOS = /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;



        // צחי שינה כדי שיעבוד ללא הפיירבס 
        //  if (!iOS) {
        ////  if (iOS) {
        //       // Initialize Firebase
        //       var config = {
        //           apiKey: "AIzaSyAGKOIfx7E5O_9JqUju_-AmjEM-4w20hw0",
        //           authDomain: "test-ba446.firebaseapp.com",
        //           databaseURL: "https://test-ba446.firebaseio.com",
        //           storageBucket: "test-ba446.appspot.com",
        //           messagingSenderId: "386031058381"
        //       };
        //       firebase.initializeApp(config);

        //       const messaging = firebase.messaging();
        //       messaging.requestPermission()
        //           .then(function () {
        //               console.log('Notification permission granted.');
        //               return messaging.getToken();
        //           })
        //           .then(function (token) {
        //               $rootScope.$broadcast('fcmToken', token);
        //           })
        //           .catch(function (err) {

        //               console.log('Unable to get permission to notify.', err);
        //           });

        //       messaging.onMessage(function (payload) {
        //           $rootScope.$broadcast('fcmMessage', payload);
        //       });

        //       messaging.onTokenRefresh(function () {
        //           messaging.getToken()
        //               .then(function (refreshedToken) {
        //                   $rootScope.$broadcast('fcmToken', refreshedToken);
        //               })
        //               .catch(function (err) {
        //                   console.log('Unable to retrieve refreshed token ', err);
        //               });
        //       });

        //       $rootScope.$on('fcmToken', function (event, token) {
        //           $http.post(sharedValues.apiUrl + 'notifications/register/', '"' + token + '"').then(function () {
        //               localStorage.setItem('deviceToken', token);
        //           });
        //       });

        //       $rootScope.$on('fcmMessage', function (event, payload) {
        //           alert(payload.notification.body);
        //       });
        //   }

        //$rootScope.$on("$stateChangeError", console.log.bind(console));
        //$rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {

        //    if (fromState.name == 'horse' || fromState.name == 'student') {
        //        $rootScope.$broadcast('submit');
        //    }

        //    console.log('$stateChangeStart to ' + toState.to + '- fired when the transition begins. toState,toParams : \n', toState, toParams);
        //    $rootScope.$broadcast('commonOperations');
        //});
        //$rootScope.$on('$stateChangeError', function (event, toState, toParams, fromState, fromParams, error) {
        //    console.log('$stateChangeError - fired when an error occurs during transition.');
        //    console.log(arguments);
        //});
        //$rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
        //    console.log('$stateChangeSuccess to ' + toState.name + '- fired once the state transition is complete.');
        //});
        //$rootScope.$on('$viewContentLoaded', function (event) {
        //    console.log('$viewContentLoaded - fired after dom rendered', event);
        //    notificationsService.getNotifications().then(function (data) {

        //        $rootScope.$broadcast('notificationsNav.refresh', data.length)
        //    });
        //});
        $rootScope.$on('$stateNotFound', function (event, unfoundState, fromState, fromParams) {
            console.log('$stateNotFound ' + unfoundState.to + '  - fired when a state cannot be found by its name.');
            console.log(unfoundState, fromState, fromParams);
        });


    });

    if (window.location.hostname.indexOf('giddyup') != -1 && window.location.protocol != 'https:') {
        window.location.href = 'https://www.giddyup.co.il';
    }

    app.directive('signaturePad', [
        '$window',
        function ($window) {
            'use strict';

            var signaturePad,
                canvas,
                element,
                EMPTY_IMAGE = 'data:image/gif;base64,R0lGODlhAQABAAAAACwAAAAAAQABAAA=';
            return {
                restrict: 'EA',
                replace: true,
                template: '<div class="signature"><canvas id="maincanvas"></canvas></div>',
                scope: {
                    accept: '=',
                    clear: '=',
                    dataurl: '=',
                    height: '@',
                    width: '@',
                },
                controller: [
                    '$scope',
                    function ($scope) {
                        $scope.accept = function () {
                            var signature = {};

                            if (!$scope.signaturePad.isEmpty()) {
                                signature.dataUrl = $scope.signaturePad.toDataURL();
                                signature.isEmpty = false;
                            } else {
                                signature.dataUrl = EMPTY_IMAGE;
                                signature.isEmpty = true;
                            }

                            return signature;
                        };

                        $scope.clear = function () {
                            $scope.signaturePad.clear();
                        };

                        $scope.$watch('dataurl', function (dataUrl) {
                            if (dataUrl) {
                                $scope.signaturePad.fromDataURL(dataUrl);
                            }
                        });
                    },
                ],
                link: function (scope, element) {
                    canvas = element.find('canvas')[0];

                    scope.onResize = function () {


                        var canvas = element.find('canvas')[0];

                        // canvas.font = "120px Arial";
                        var ratio = Math.max($window.devicePixelRatio || 1, 1);

                        canvas.width = canvas.offsetWidth * ratio;
                        canvas.height = canvas.offsetHeight * ratio;
                        canvas.getContext('2d').scale(ratio, ratio);

                        //   canvas.getContext('2d').font = "120px Arial";



                    };

                    scope.onResize();

                    scope.signaturePad = new SignaturePad(canvas);

                    angular.element($window).bind('resize', function () {
                        scope.onResize();
                    });
                },
            };
        },
    ]);

})();