var dataTable;

$(document).ready(function () {
    //$-shortcut for document.getElementById()- https://www.thoughtco.com/and-in-javascript-2037515
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll" 
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                /*account lock and unlock features - this property is provided by ASP.NETCore Entity Framework
                 *under users Table (dbo.AspNetUsers table)- LockoutEnd property(column) : This sets the
                 *date for which account remains locked*/
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //user is currently locked. So we need to unlock
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" 
                                    style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock-open"></i> Unlock
                                </a>
                            </div>
                               `;
                    }
                    else {
                        //user is currently unlocked. So we need to lock
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" 
                                    style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock"></i> Lock
                                </a>
                            </div>
                               `;
                    }
                }, "width": "25%"
            }
        ]
    }); //semicolon - must (don't forget)
}
function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock', //API Call
        data: JSON.stringify(id),
        contentType:"application/json",
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