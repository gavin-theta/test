function accordion() {
    $('.collapse-trigger').click(function () {
        $(this).parents('.collapse-container').toggleClass('collapsed');

        let collapsecontent = $(this).parents('.collapse-container');

        if (collapsecontent.data) {
            var collapsecontentId = collapsecontent.data('collapsecontent');
            if (collapsecontentId) {
                $(`#${collapsecontentId}`).toggleClass('visibility-none');
            }
        }
    });

    $('.collapse-trigger').keypress(function (e) {
        let keyCode = keyboard.getKeyCode(e);
        if (keyCode === 13) {

            $(this).parents('.collapse-container').toggleClass('collapsed');
            let collapsecontent = $(this).parents('.collapse-container');
            if (collapsecontent.data) {
                var collapsecontentId = collapsecontent.data('collapsecontent');
                if (collapsecontentId) {
                    $(`#${collapsecontentId}`).toggleClass('visibility-none');
                }
            }

          //  setCollapsedContent();
        }
    });
}

function setCollapsedContent() {
    $('.collapse-container.collapsed').each(function (i, obj) {
        if (obj.data) {
            let collapsecontentId = obj.data('collapsecontent');
            if (collapsecontentId) {
                $(`#${collapsecontentId}`).addClass('visibility-none');
            }
        }
    })
}

function notifCollapse() {
    var nTrigger = $('.notifications-trigger');
    var nPanel = $('.notification-panel');
    nTrigger.click(function(){
        nPanel.toggleClass('collapsed');
    });
    $(document).mouseup(function(e) {
        if (!nPanel.is(e.target) && nPanel.has(e.target).length === 0) 
        {
            nPanel.addClass('collapsed');
        }
    });
}

$(document).ready(function () {
    accordion();
    setCollapsedContent();
    notifCollapse();
    window.onscroll = function () {
        const header = $('#header');
        if (document.body.scrollTop > 80 || document.documentElement.scrollTop > 80) {
            header.addClass('scrolled');
        } else {
            header.removeClass('scrolled');
        }
    };

    //enable keyboard nav
    document.body.addEventListener('keyup', function (e) {
        if (e.which === 9) /* tab */ {
            document.documentElement.classList.remove('no-focus-outline');
        }
    });
});


// date
Date.prototype.today = function () {
    return (this.getDate() < 10 ? "0" : "") + this.getDate() + "/" + (this.getMonth() + 1 < 10 ? "0" : "") + (this.getMonth() + 1) + "/" + this.getFullYear();
};

// time 
Date.prototype.timeNow = function () {
    var hours = this.getHours();
    var minutes = this.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    return hours + ':' + minutes + ' ' + ampm;
};