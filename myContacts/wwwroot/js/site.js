// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function RedirectToLogin()
{
    document.location = "/";
}

function GetAPIToken()
{
    return localStorage.getItem("jwt_mycontacts");
}

function parseJwt(token)
{
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c)
    {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
};

function GetUsername()
{
    let parsedToken = parseJwt(GetAPIToken());
    return parsedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
}

function SetAPIToken(username, token)
{
    localStorage.setItem('jwt_mycontacts', token);
}

function DeleteAPIToken()
{
    localStorage.removeItem('jwt_mycontacts');
}

function ApplyTokenBeforeSend(xhr)
{
    xhr.setRequestHeader('Authorization', "Bearer " + GetAPIToken());
}

function OnAPIFail(xhr, textStatus, errorThrown)
{
    if (xhr.status == 401)
    {
        DeleteAPIToken();
        RedirectToLogin();
    }
}