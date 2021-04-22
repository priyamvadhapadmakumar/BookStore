// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//https://www.youtube.com/watch?v=3r6RfShv8m8 -- Refered this video
//***********DIDN'T WORK ******************//
ShowAsPopUp = (url, title) => {
    $.ajax({
        type: 'GET', //this returns a form
        url: url,
        //to render the GET form into a html inside modal pop-up modal body (_layout.cshtml)
        success: function (response) {
            $("#form-modal .modal-body").html(response);
            $("#form-modal .modal-title").html(title);
            $("#form-modal").modal('show');
            //to make draggable pop-up
            $('.modal-dialog').draggable({
                handle: ".modal-header"
            });
        }
    })
}
