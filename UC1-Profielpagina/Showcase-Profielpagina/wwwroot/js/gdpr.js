class GDPR {

    constructor() {
        if (document.getElementById('content-gpdr-consent-status')) {
            this.showStatus();
            this.showContent();
        }
        this.bindEvents();
        if (this.cookieStatus() !== 'accept') this.showGDPR();
    }

    bindEvents() {
        let buttonAccept = document.querySelector('.gdpr-consent__button--accept');
        buttonAccept.addEventListener('click', () => {
            this.cookieStatus('accept');
            if (document.getElementById('content-gpdr-consent-status')) {
                this.showStatus();
                this.showContent();
            }
            this.hideGDPR();
        });
        let buttonDecline = document.querySelector(".gdpr-consent__button--reject");
        buttonDecline.addEventListener("click", () => {
            this.cookieStatus("reject");
            if (document.getElementById('content-gpdr-consent-status')) {
                this.showStatus();
                this.showContent();
            }
            this.hideGDPR();
        })

    }

    showContent() {
        this.resetContent();
        const status = this.cookieStatus() == null ? 'not-chosen' : this.cookieStatus();
        const element = document.querySelector(`.content-gdpr-${status}`);
        element.classList.add('show');

    }

    resetContent(){
        const classes = [
            '.content-gdpr-accept',

            ".content-gdpr-reject",

            '.content-gdpr-not-chosen'];

        for(const c of classes){
            document.querySelector(c).classList.add('hide');
            document.querySelector(c).classList.remove('show');
        }
    }

    showStatus() {
        document.getElementById('content-gpdr-consent-status').innerHTML =
            this.cookieStatus() == null ? 'Niet gekozen' : this.cookieStatus();
    }

    cookieStatus(status) {
        if (status) {
            document.cookie = `gdpr-consent-choice=${status};path=/`;
        }
        return this.getCookie("gdpr-consent-choice");
    }

    getCookie(key) {
        return document.cookie
            .split(";")
            .map(cookie => cookie.trim())
            .find(cookie => cookie.startsWith(`${key}=`))
            ?.split("=")[1] ?? null;
    }


    hideGDPR(){
        document.querySelector(`.gdpr-consent`).classList.add('hide');
        document.querySelector(`.gdpr-consent`).classList.remove('show');
    }

    showGDPR(){
        document.querySelector(`.gdpr-consent`).classList.add('show');
    }

}

const gdpr = new GDPR();

