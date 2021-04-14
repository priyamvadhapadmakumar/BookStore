var dataTable;

$(document).ready(function () {
    loadDataTable();
});

//https://datatables.net/manual/ajax - refer this document
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url:'/Admin/Book/GetAll'
        },
        columns: [
            { "data": "title", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            {
                "data": "bookId",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/Book/Upsert/${data}" data-toggle="tooltip" title="Edit" 
                                   class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i>
                                </a>
                                <a onclick=Delete("/Admin/Book/Delete/${data}")
                                   data-toggle="tooltip" title="Delete"
                                   class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="fas fa-trash-alt"></i>
                                </a>
                            </div>
                       `;
                }, "width": "10%"
            }
        ]
    });
}
function Delete(url) {
    //using sweetAlert for alert messages
    swal({// see eg in https://sweetalert.js.org/guides/
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}