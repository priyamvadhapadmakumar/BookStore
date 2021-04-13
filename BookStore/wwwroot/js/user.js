var dataTable;

$(document).ready(function () {
    //$-shortcut for document.getElementById()- https://www.thoughtco.com/and-in-javascript-2037515
    loadDataTable();
});

//https://datatables.net/manual/ajax - refer this document
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url:'/Admin/User/GetAll' 
        },
        columns: [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "role", "width": "15%" }
        ]
    }); //semicolon - must (don't forget)
}
