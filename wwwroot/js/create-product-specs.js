// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("#add").on("click", function () {
        var rowIndex = $(this).closest('tr').prevAll().length;
        var $row = $("#tab tbody tr").first().clone();
        $row.find("input").val("")
            .attr('id', 'spec_id' + rowIndex)
            .attr('name', 'spec_name' + rowIndex);
        $("#tab tbody").append($row);
    });
});