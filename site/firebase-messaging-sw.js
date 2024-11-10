importScripts('https://www.gstatic.com/firebasejs/3.6.1/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/3.6.1/firebase-messaging.js');

// Initialize Firebase
var config = {
    apiKey: "AIzaSyAGKOIfx7E5O_9JqUju_-AmjEM-4w20hw0",
    authDomain: "test-ba446.firebaseapp.com",
    databaseURL: "https://test-ba446.firebaseio.com",
    storageBucket: "test-ba446.appspot.com",
    messagingSenderId: "386031058381"
};
firebase.initializeApp(config);

const messaging = firebase.messaging();

messaging.setBackgroundMessageHandler(function (payload) {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    // Customize notification here
    const notificationTitle = payload.notification.title;
    const notificationOptions = {
        body: payload.notification.body
    };

    return self.registration.showNotification(notificationTitle,
        notificationOptions);
});