window.MFPortal = {
  SubmitForm: function (element) {
    element.submit();
  },
  LocalTime: function () {
    return new Date().getTimezoneOffset();
  },
  LocalDate: function () {
    return new Date().toString();
  },
  GoogleCaptcha: function (dotNetObject, selector, sitekeyValue) {
    return grecaptcha.render(selector, {
      'sitekey': sitekeyValue,
      'callback': (response) => { dotNetObject.invokeMethodAsync('CallbackOnSuccess', response); },
      'expired-callback': () => { dotNetObject.invokeMethodAsync('CallbackOnExpired'); }
    });
  },
  DisposeKendo: function destroyWidgets(container) {
    if (!this.EnsureKendo()) { return; }
    kendo.destroy(container);
  },
  EnsureKendo: function ensureKendoAndJquery() {
    if (!window.$ || !window.kendo) {
      alert("Something went wrong with loading the Kendo library, review the script references.");
      return false;
    }
    return true;
  },
  KendoInitialize: function createWidget(container, id, dotNetComponent, initialColor) {
    $.ajaxPrefilter(function (options) {
      if (options.type === 'GET' && options.dataType === 'script') {
        options.cache = true;
      }
    });

    this.ViewPDF($(container).find("#" + id), dotNetComponent, initialColor);
  },
  ViewPDF: function createPdfViewer($elem) {
    $.when(
      $elem.text("Loading ..."),
      $.getScript("https://kendo.cdn.telerik.com/2021.3.1207/js/kendo.all.min.js"),
      $.getScript("https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.2.2/pdf.min.js"),
      $.getScript("https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.2.2/pdf.worker.min.js")
    ).done(function () {
      $elem.text("Preparing View ..."),
        window.pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.2.2/pdf.worker.min.js';
    }).then(function () {
      $elem.text(""),
        $elem.kendoPDFViewer({
          pdfjsProcessing: {
            file: $elem[0].dataset.filename
          },

          width: $elem[0].dataset.width,
          height: $elem[0].dataset.height
        }).data("kendoPDFViewer");
    });
  }
}
