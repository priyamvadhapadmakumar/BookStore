var dataTable;

$(document).ready(function () {
    //$-shortcut for document.getElementById()- https://www.thoughtco.com/and-in-javascript-2037515
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/CoverType/GetAll" //Area/ControllerName/Action method or API
        },
        "columns": [
            { "data": "name", "width": "60%" },
            //naming inside datatable is lower camelcase. So, Name starts with 'n'
            {
                "data": "id",
                //render 2 buttons/2 links(upsert and delete)
                "render": function (data) {
                    //return `(tie operator) used to return a div
                    //first type the div portion in Index.cshtml to avoid spelling mistakes and check working
                    //href="Admin/Category/Upsert/id(where we pass the data)
                    return `
                            <div class="text-center">
                                <a href="/Admin/CoverType/Upsert/${data}" data-toggle="tooltip" title="Edit" 
                                   class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i> &nbsp;
                                </a>
                                <a onclick=Delete("/Admin/CoverType/Delete/${data}")
                                   data-toggle="tooltip" title="Delete"
                                   class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="fas fa-trash-alt"></i> &nbsp;
                                </a>
                            </div>
                       `;
                }, "width": "40%"
            }
        ]
    }); 
}
function Delete(url) {
    swal({
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