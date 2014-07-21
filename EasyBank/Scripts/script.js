$(document).ready(function () {
    $("#langSelect").change(
    function () {
        $(this).closest('form').trigger('submit');
    });
});