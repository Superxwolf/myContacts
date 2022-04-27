const validation = new JustValidate('#contactInfo', {
    errorsContainer: "#ModalErrorMessage"
});

function ParseDate(time)
{
    var updateDate = new Date(time);

    let timeStr = updateDate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true });
    let dateStr = updateDate.toLocaleString('en-US', { day: '2-digit', month: '2-digit', year: "numeric" });

    return dateStr + " " + timeStr;
}

function ParsePhone(phone)
{
    if (!phone) return "";
    phone = phone.trim();
    if (phone.length != 10) return "";
    return `(${phone.substring(0, 3)}) ${phone.substring(3, 6)}-${phone.substring(6, 10)}`;
} 

function FetchContacts()
{
    let url = '/api/contact';
    let data = undefined;

    var search = $("#SearchFormInput").val().trim();

    if(search && search.length > 0)
    {
        url = '/api/contact/search';
        data = { 'search': search };
    }

    $("#LoaderIndicator").show();

    $.ajax({
            "url": url,
            "dataType": "json",
            "contentType": "application/json",
            "data": data,
            "beforeSend": ApplyTokenBeforeSend
        })
        .always(() =>
        {
            $("#LoaderIndicator").hide();
        })
        .done(data =>
        {
            if (Array.isArray(data))
            {
                let $table = $("#contactsTable tbody");

                $table.empty();

                data.forEach(entry =>
                {
                    $table.append(`
                        <tr>
                            <td><a href="#" onclick="EditContact(${entry.contactID})">Edit</a></td>
                            <td>${entry.name}</td>
                            <td>${ParsePhone(entry.phone)}</td>
                            <td>${ParsePhone(entry.fax)}</td>
                            <td>${entry.eMail}</td>
                            <td>${ParseDate(entry.lastUpdateDate)}</td>
                        </tr>`);
                });
            }
            else
            {
                console.error(data);
            }
        })
        .fail(OnAPIFail);
}

function AddContact()
{
    ClearModalTextBoxes();
    $("#ContactModal").show();
    EnableTextBoxes();
}

function EditContact(ContactID)
{
    ClearModalTextBoxes();
    $("#ContactModal").show();
    FetchContactModal(ContactID);
}

function SaveContactModal()
{
    DisableTextBoxes();

    validation.revalidate().then(isValid =>
    {
        if (!isValid)
        {
            EnableTextBoxes();
            return;
        }

        let contact = {
            name: $("#Name").val(),
            phone: $("#Phone").inputmask('unmaskedvalue'),
            fax: $("#Fax").inputmask('unmaskedvalue'),
            eMail: $("#eMail").val(),
            notes: $("#Notes").val()
        }

        let url = '/api/contact/';
        let method = "PUT";
        let creatingContact = true;

        let contactId = $("#ContactId").val();

        if (contactId > 0)
        {
            contact.contactID = contactId;
            url = '/api/contact/' + contactId;
            method = "POST";
            creatingContact = false;
        }

        $("#LoaderIndicator").show();

        $.ajax({
            url: url,
            method: method,
            contentType: 'application/json',
            data: JSON.stringify(contact),
            beforeSend: ApplyTokenBeforeSend
        })
            .always(() =>
            {
                $("#LoaderIndicator").hide();
            })
            .done(data =>
            {
                console.log("Update finished");
                //EnableTextBoxes();
                CloseContactModal();
                FetchContacts();
                Toastify({
                    text: (creatingContact ? "Contact Created" : "Contact Saved"),
                    duration: 3000,
                    close: true,
                    gravity: "bottom",
                    position: "right",
                    stopOnFocus: true,
                    style: {
                        background: "linear-gradient(to right, #00b09b, #96c93d)",
                    },
                    onClick: function () { } // Callback after click
                }).showToast();
            })
            .fail((xhr) =>
            {
                if (xhr.status == 400)
                {
                    EnableTextBoxes();
                }
            })
            .fail(OnAPIFail);
    });
}

function CloseContactModal()
{
    $("#ContactModal").hide();
}

function FetchContactModal(id)
{
    $("#ContactId").val(id);

    $("#LoaderIndicator").show();

    $.ajax({
            url: '/api/contact/' + id,
            dataType: 'json',
            beforeSend: ApplyTokenBeforeSend
        })
        .always(() =>
        {
            $("#LoaderIndicator").hide();
        })
        .done(data =>
        {
            if (typeof (data) == "object")
            {
                ClearModalTextBoxes();

                $("#ContactId").val(data.contactID);

                $("#Name").val(data.name.trim());
                $("#Phone").val(data.phone.trim());

                if(data.fax)
                    $("#Fax").val(data.fax.trim());

                if(data.eMail)
                    $("#eMail").val(data.eMail.trim());

                if(data.notes)
                    $("#Notes").val(data.notes.trim());

                EnableTextBoxes();
            }
        });
}

function EnableTextBoxes()
{
    $("#contactInfo input").attr("disabled", null);
    $("#contactInfo textarea").attr("disabled", null);
}

function DisableTextBoxes()
{
    $("#contactInfo input").attr("disabled", "disabled");
    $("#contactInfo textarea").attr("disabled", "disabled");
}

function ClearModalTextBoxes()
{
    $("#ModalErrorMessage").html("");
    $("#ContactId").val(0);
    $("#Name").val("");
    $("#Phone").val("");
    $("#Fax").val("");
    $("#eMail").val("");
    $("#Notes").val("");
}

function Logout()
{
    $.ajax({
        url: '/api/logout',
        beforeSend: ApplyTokenBeforeSend
    }).always(() =>
    {
        DeleteAPIToken();
        RedirectToLogin();
    });
}

FetchContacts();
$("#UsernameDisplay").html(GetUsername());

$("#ContactModal").draggable({
    handle: "#ModalDragHandle",
    containment: "document",
    cursor: "crosshair"
});

$("#contactInfo :input").inputmask();

validation
    .addField("#Name", [
        {
            rule: 'required',
            errorMessage: "Username is required!"
        },
        {
            rule: 'maxLength',
            value: 50,
            errorMessage: "Username must be 50 characters or less"
        }
    ])
    .addField("#Phone", [
        {
            validator: (value) =>
            {
                var numbers = value.replace(/\D/g, "");
                return numbers.length == 10;
            },
            errorMessage: "A valid phone is required"
        }
    ])
    .addField("#Fax", [
        {
            validator: (value) =>
            {
                var numbers = value.replace(/\D/g, "");
                return numbers.length == 10 || numbers.length == 0;
            },
            errorMessage: "Must be a valid fax number"
        }
    ])
    .addField("#eMail", [
        {
            validator: (value) =>
            {
                if (value.length == 0) return true;
                else if (value.length > 50) return false;
                
                return value.toLowerCase()
                .match(
                    /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
                );
            },
            errorMessage: 'Not a valid email address'
        }
    ]);

$("#SearchFormInput").on('keyup', function (event)
{
    if (event.keyCode == 13)
    {
        $("#SearchFormBtn").click();
    }
});