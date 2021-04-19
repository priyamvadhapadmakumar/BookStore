var dataTable;

$(document).ready(function () {
    loadDataTable();
});

//https://datatables.net/manual/ajax - refer this document
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url:'/Admin/Inventory/GetAll'
        },
        columns: [
            { "data": "book.title" },
            { "data": "count" },
            {
                "data": "inventoryId",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/Inventory/Upsert/${data}" data-toggle="tooltip" title="Edit" 
                                   class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i>
                                </a>
                            </div>
                       `;
                }, "width": "15%"
            }
        ]
    });
}
