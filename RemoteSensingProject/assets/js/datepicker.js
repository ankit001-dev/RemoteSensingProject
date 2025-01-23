$(function() {
  'use strict';

    if ($('.datePickerExample').length) {
        debugger
    var date = new Date();
    var today = new Date(date.getFullYear(), date.getMonth(), date.getDate());
        $('.datePickerExample').datepicker({
      format: "dd-MM-yyyy",
      todayHighlight: true,
      autoclose: true
    });
        $('.datePickerExample').datepicker('setDate', today);
  }
});