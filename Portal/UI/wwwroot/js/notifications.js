var notifications = (function () {
    let heartbeatStatus = 1;

    const toastPriority = ['toast toast-info', 'toast toast-success', 'toast toast-warning', 'toast toast-error'];
    const empty = new Array();
    const toastAndNotification = 0;
    const toastOnly = 2;
    const notificationOnly = 1;
  
    function getStorage() {
        let storageData = sessionStorage.getItem("notifications");
        if (!storageData || !storageData.length) {
            storageData = JSON.stringify(empty);
        }

        return JSON.parse(storageData) || empty;
    }

    function setStorage(notifications) {
        sessionStorage.setItem("notifications", JSON.stringify(notifications));
    }

    function clearStorage() {
        setStorage(empty);
        setNotificationCount();
    }
    
    function renderNotification(notification) {
        const date = new Date();
        let now  = date.today() + " " + date.timeNow();
        let newNotification = '<div id="panel_' + notification.id + '" class="' + toastPriority[notification.priority] + ' position: absolute; top: 0; right: 0;" role="alert" aria-live="assertive" aria-atomic="true">' +
            '<div class="toast-header">' +
            '<strong class="mr-auto"><i class="fa fa-grav"></i>&nbsp;' + notification.title + '</strong>' +
            '<small class="text-muted">' + now + '</small>' +
            '<button id="panel_remove_' + notification.id + '" type="button" class="ml-2 close" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
            '</div >' +
            '<div class="toast-body">' + notification.message + '</div>' +
            '</div >';

        $("#notifications-holder").prepend(newNotification);
        $("#panel_remove_" + notification.id).click(function () { removeNotification(notification.id, true); });
    }

    function removeNotification(id, notify) {
        $("#panel_" + id).fadeOut(500);
     
        const storage = getStorage();

        const victim = storage.filter(function (notification) {
                return notification.id === id;
        });

        if (victim[0].title === "Heartbeat") {
            heartbeatStatus = 1;
        }

        const filtered = storage.filter(function (notification) {
            return notification.id !== id;
        });

        setStorage(filtered);

        (function (id) {
            setTimeout(function () {
                $(id).remove();
                setNotificationCount(); 
        }, 500);
        })("#panel_" + id);
        
    }

    function showToast(notification) {
        const date = new Date();
        let now = date.today() + " " + date.timeNow();
        let newToast = '<div id="toast_' + notification.id + '" class="' + toastPriority[notification.priority] + '" role="alert" aria-live="assertive" aria-atomic="true">' +
            '<div class="toast-header">' +
            '<strong class="mr-auto"><i class="fa fa-grav"></i>&nbsp;' + notification.title + '</strong>' +
            '<small class="text-muted">' + now + '</small>' +
            '<button id="notif-close_' + notification.id + '" type="button" class="ml-2 close" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
            '</div >' +
            '<div class="toast-body">' + notification.message + '</div>' +
            '</div >';
        $("#toast-holder").prepend(newToast);

        $("#notif-close_" + notification.id).click(function () { $("#toast_" + notification.id).fadeOut(500); });

        hideToast(notification.id);
    }

    function hideToast(notificationId) {
        (function (id) {
            setTimeout(function () {
                $(id).fadeOut(500).remove();

            }, 9000);
        })("#toast_" + notificationId);
       
    }

    function setNotificationCount() {
        const notifications = getStorage() || [];
        const count = notifications.length;
        if (count > 0) {
            $("#notif-counter").text("" + count + "");
            $("#notif-counter").removeClass('hidden');
            $(".notifications-empty").addClass('hidden');
            $(".notifications-clear").removeClass('hidden');
        } else {
            $(".notifications-clear").addClass('hidden');
            $(".notifications-empty").removeClass('hidden');
            $("#notif-counter").addClass('hidden');
            $("#notif-counter").text("");
            $(".notification-panel").addClass('collapsed');
        }
    }

    function addNotification(title, message, priority, display) {
        //mode 0 notify&toast  1 notify only 2 toast only
        const date = new Date();
        const id = date.getTime();

        const newNotification = {
            "id": id,
            "priority": priority,
            "title": title,
            "message": message,
            "time": date.today() + " " + date.timeNow(),
            "display": display
        };
       
        showNotification(newNotification);
    }

    //Public
    function addInfo(title, message, display) {
        addNotification(title, message, 0, display);
    }

    function addSuccess(title, message, display) {
        addNotification(title, message, 1, display);
    }

    function addWarning(title, message, display) {
        addNotification(title, message, 2, display);
    }

    function addError(title, message, display) {
        addNotification(title, message, 3, display);
    } 

    function showNotification(notification) {
        let displayMode = notification.display || toastAndNotification;
        if (displayMode === toastAndNotification || displayMode === notificationOnly) {
            let notifications = getStorage();
            notifications.push(notification);
            setStorage(notifications);
            renderNotification(notification);            
        }
        if (displayMode === toastAndNotification || displayMode === toastOnly) {
            showToast(notification);
        } 
        setNotificationCount();
    }

    function showHeartbeatNotification(notification) {

        let sts = document.getElementById("_heartbeat");

        if (sts) {
            let time = utility.formatLongDate(notification.time);
            sts.innerHTML = time;
            $("#_heartbeat").removeClass("badge-success badge-danger badge-warning");
            if (notification.priority === 1) { $("#_heartbeat").addClass("badge-success") }
            if (notification.priority > 1) {
                $("#_heartbeat").addClass("badge-danger");
            }
        }

        if (heartbeatStatus !== notification.priority) {
            heartbeatStatus = notification.priority;
            
            let displayMode = notification.display || toastAndNotification;
            if (displayMode === toastAndNotification || displayMode === notificationOnly) {
                let notifications = getStorage();
                notifications.push(notification);
                setStorage(notifications);
                renderNotification(notification);
            }
            if (displayMode === toastAndNotification || displayMode === toastOnly) {
                showToast(notification);
            }
            setNotificationCount();
        } else {
            if (notification.priority === 3) {
                //allways show toast for error!
                showToast(notification);
            }
        }

    }

    function clearAll(notify) {
        const notifications = getStorage() || [];
        notifications.forEach(function (element) {
            removeNotification(element.id, notify);
        });
        clearStorage();
        heartbeatStatus = 1;
    }

    function init() {
        const notifications = getStorage() || [];
        setNotificationCount();

        notifications.forEach(function (element) {
            renderNotification(element);
        });
    }

    return {
        addInfo: addInfo,
        addWarning: addWarning,
        addError: addError,
        addSuccess: addSuccess,
        show: showNotification,
        showHeartbeat: showHeartbeatNotification,
        clear: clearAll,
        remove: removeNotification,        
        toastAndNotification: toastAndNotification,
        toastOnly: toastOnly,
        notificationOnly: notificationOnly,
        init: init
    };

})();

