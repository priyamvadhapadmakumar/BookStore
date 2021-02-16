var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        //DataTable will only be recongnized if we add the corresponding javascript ref in _Layout.cshtml
        /*Refer https://skillcrush.com/blog/what-is-ajax/ for understanding ajax*/
        "ajax": {
            "url": "/Admin/Company/GetAll" //Area/ControllerName/Action method or API
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "address", "width": "15%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "15%" },
            { "data": "contactNumber", "width": "10%" },
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox" disabled checked />`
                    }
                    else {
                        return`<input type="checkbox" disabled />`
                    }
                },
                "width":"10%"
            },
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
                                <a href="/Admin/Company/Upsert/${data}" data-toggle="tooltip" title="Edit" 
                                   class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i> &nbsp;
                                </a>
                                <a onclick=Delete("/Admin/Company/Delete/${data}")
                                   data-toggle="tooltip" title="Delete"
                                   class="btn btn-danger text-white" style="cursor:pointer">
                                    <i class="fas fa-trash-alt"></i> &nbsp;
                                </a>
                            </div>
                       `;//semicolon must
                }, "width": "25%"
            }
        ]
    }); //semicolon - must (don't forget)
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