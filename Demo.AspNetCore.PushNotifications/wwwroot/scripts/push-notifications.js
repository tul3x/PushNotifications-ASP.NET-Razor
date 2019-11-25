const PushNotifications = (function () {
    let applicationServerPublicKey;

    let consoleOutput;
    let pushServiceWorkerRegistration;
    let subscribeButton, unsubscribeButton;
    let topicInput, urgencySelect, notificationInput;

    function initializeConsole() {
        consoleOutput = document.getElementById('output');
        document.getElementById('clear').addEventListener('click', clearConsole);
    }

    function clearConsole() {
        while (consoleOutput.childNodes.length > 0) {
            consoleOutput.removeChild(consoleOutput.lastChild);
        }
    }

    function writeToConsole(text) {
        var paragraph = document.createElement('p');
        paragraph.style.wordWrap = 'break-word';
        paragraph.appendChild(document.createTextNode(text));

        consoleOutput.appendChild(paragraph);
    }

    function registerPushServiceWorker() {
        navigator.serviceWorker.register('/scripts/service-workers/push-service-worker.js', { scope: '/scripts/service-workers/push-service-worker/' })
            .then(function (serviceWorkerRegistration) {
                pushServiceWorkerRegistration = serviceWorkerRegistration;

                initializeUIState();

                writeToConsole('Push Service Worker erfolgreich registriert');
            }).catch(function (error) {
                writeToConsole('Push Service Worker Fehler: ' + error);
            });
    }

    function initializeUIState() {
        subscribeButton = document.getElementById('subscribe');
        subscribeButton.addEventListener('click', subscribeForPushNotifications);

        notificationInput = document.getElementById('notification');
        document.getElementById('send').addEventListener('click', sendPushNotification);

        pushServiceWorkerRegistration.pushManager.getSubscription()
            .then(function (subscription) {
                changeUIState(Notification.permission === 'denied', subscription !== null);
            });
    }

    function changeUIState(notificationsBlocked, isSubscibed) {
        subscribeButton.disabled = notificationsBlocked || isSubscibed;

        if (notificationsBlocked) {
            writeToConsole('Berechtigung zum senden der Push-Nachrichten abgelehnt');
        }
    }

    function subscribeForPushNotifications() {
        if (applicationServerPublicKey) {
            subscribeForPushNotificationsInternal();
        } else {
            PushNotificationsController.retrievePublicKey()
                .then(function (retrievedPublicKey) {
                    applicationServerPublicKey = retrievedPublicKey;
                    writeToConsole('Public Key erhalten');

                    subscribeForPushNotificationsInternal();
                }).catch(function (error) {
                    writeToConsole('Fehler beim Erhalten des Public Key: ' + error);
                });
        }
    }

    function subscribeForPushNotificationsInternal() {
        pushServiceWorkerRegistration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: applicationServerPublicKey
        })
            .then(function (pushSubscription) {
                PushNotificationsController.storePushSubscription(pushSubscription)
                    .then(function (response) {
                        if (response.ok) {
                            writeToConsole('Push-Abonnement erfogreich auf dem Server gespeichert');
                        } else {
                            writeToConsole('Fehler beim Speichern des Abonnements auf dem Server');
                        }
                    }).catch(function (error) {
                        writeToConsole('Fehler beim Speichern des Abonnements auf dem Server: ' + error);
                    });

                changeUIState(false, true);
            }).catch(function (error) {
                if (Notification.permission === 'denied') {
                    changeUIState(true, false);
                } else {
                    writeToConsole('Fehler beim Speichern des Abonnements: ' + error);
                }
            });
    }

    function sendPushNotification() {
        let payload = { notification: notificationInput.value };

        fetch('push-notifications-api/notifications', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        })
            .then(function (response) {
                if (response.ok) {
                    writeToConsole('Nachrricht erfolgreich an der Server gesendet');
                } else {
                    writeToConsole('Fehler beim Senden der Nachricht');
                }
            }).catch(function (error) {
                writeToConsole('Fehler beim Senden der Nachricht: ' + error);
            });
    }

    return {
        initialize: function () {
            initializeConsole();

            if (!('serviceWorker' in navigator)) {
                writeToConsole('Service Worker nicht unterstützt');
                return;
            }

            if (!('PushManager' in window)) {
                writeToConsole('Push API nicht unterstützt');
                return;
            }

            registerPushServiceWorker();
        }
    };
})();

PushNotifications.initialize();