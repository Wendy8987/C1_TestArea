if (document.getElementById("Permission").innerHTML == "0") {
    document.getElementById("Login").style.display = "block";
    document.getElementById("Register").style.display = "block";
    document.getElementById("Profile").style.display = "none";
    document.getElementById("UploadModel").style.display = "none";
    document.getElementById("UploadedModel").style.display = "none";
    document.getElementById("ModelManagement").style.display = "none";
    document.getElementById("AccountManagement").style.display = "none";
    document.getElementById("Logout").style.display = "none";

    document.getElementById("Login_").style.display = "block";
    document.getElementById("Register_").style.display = "block";
    document.getElementById("Profile_").style.display = "none";
    document.getElementById("UploadModel_").style.display = "none";
    document.getElementById("UploadedModel_").style.display = "none";
    document.getElementById("ModelManagement_").style.display = "none";
    document.getElementById("AccountManagement_").style.display = "none";
    document.getElementById("Logout_").style.display = "none";

} else if (document.getElementById("Permission").innerHTML == "4") {
    document.getElementById("Login").style.display = "none";
    document.getElementById("Register").style.display = "none";
    document.getElementById("Profile").style.display = "block";
    document.getElementById("UploadModel").style.display = "block";
    document.getElementById("UploadedModel").style.display = "block";
    document.getElementById("ModelManagement").style.display = "block";
    document.getElementById("AccountManagement").style.display = "block";
    document.getElementById("Logout").style.display = "block";

    document.getElementById("Login_").style.display = "none";
    document.getElementById("Register_").style.display = "none";
    document.getElementById("Profile_").style.display = "block";
    document.getElementById("UploadModel_").style.display = "block";
    document.getElementById("UploadedModel_").style.display = "block";
    document.getElementById("ModelManagement_").style.display = "block";
    document.getElementById("AccountManagement_").style.display = "block";
    document.getElementById("Logout_").style.display = "block";
} else {
    document.getElementById("Login").style.display = "none";
    document.getElementById("Register").style.display = "none";
    document.getElementById("Profile").style.display = "block";
    document.getElementById("UploadModel").style.display = "block";
    document.getElementById("UploadedModel").style.display = "block";
    document.getElementById("ModelManagement").style.display = "none";
    document.getElementById("AccountManagement").style.display = "none";
    document.getElementById("Logout").style.display = "block";

    document.getElementById("Login_").style.display = "none";
    document.getElementById("Register_").style.display = "none";
    document.getElementById("Profile_").style.display = "block";
    document.getElementById("UploadModel_").style.display = "block";
    document.getElementById("UploadedModel_").style.display = "block";
    document.getElementById("ModelManagement_").style.display = "none";
    document.getElementById("AccountManagement_").style.display = "none";
    document.getElementById("Logout_").style.display = "block";
}
function changeLanguage(lang) {
    $.ajax({
        url: '../Home/ChangeLanguage',
        type: 'POST',
        data: { lang: lang },
        success: function () {
            // 刷新页面以显示切换后的语言
            location.reload();
        },
        error: function () {
            // 处理错误
            alert('Failed to change language.');
        }
    });
}