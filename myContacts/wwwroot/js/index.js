const validation = new JustValidate('#LoginBox', {
    errorsContainer: "#LoginError"
});
function SetLoginError(message)
{
    $("#LoginError").html(message);
} 

function DoLogin()
{
    SetLoginError("");
    validation.revalidate().then(
        isValid =>
        {
            if (!isValid) return;
            
            let username = $("#username").val().trim();
            $("#LoaderIndicator").show();
            $.ajax({
                    url: '/api/login',
                    method: 'POST',
                    contentType: "application/json",
                    data: JSON.stringify({
                        "username": username
                    })
                })
                .always(() =>
                {
                    $("#LoaderIndicator").hide();
                })
                .done(data => 
                {
                    $("#LoaderIndicator").hide();
                    if (data.token)
                    {
                        console.log("Logged in");
                        SetAPIToken(username, data.token);
                        document.location = "contacts";
                    }
                    else
                    {
                        SetLoginError("Could not authenticate user");
                    }
                })
                .fail((xhr, textStatus, error) =>
                {
                    $("#LoaderIndicator").hide();
                    /*
                    console.error("Error when authenticating");
                    console.error(xhr);
                    console.error(textStatus);
                    console.error(error);
                    */
                    SetLoginError("Error while trying to authenticate user");
                });
        });
}

validation
    .addField("#username", [
        {
            rule: 'required',
            errorMessage: "Username is required!"
        },
        {
            rule: 'maxLength',
            value: 20,
            errorMessage: "Username must be 20 characters or less"
        }
    ]);