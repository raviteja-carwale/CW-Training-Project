totalUsers = 0;
numOfPages = 0;
orderFormat = '1A';
URL = 'http://172.16.3.67:60725';

$(document).ready(GetUserPage(1));

function GetUserPage(pageNo) {
    resetMsgs();
    showLoader("Loading");
    $.ajax({
        url: URL + '/api/v1/UserProfiles/?pageNo=' + pageNo + '&orderFormat=' + orderFormat,
        type: 'GET',
        dataType: 'json',
        crossDomain: true,
        success: function (data, textStatus, xhr) {
            totalUsers = parseInt(data.count);
            $.each(data.userList, function (key, item) {
                item.dateOfBirth = item.dateOfBirth.split('T')[0];
                item.gender = (item.gender == "M") ? "Male" : "Female";
            });
            self.Users(data.userList);
            highlightColumn(orderFormat[0] - 1);
            makePager(pageNo);
            hideLoader();
            if (self.Users().length == 0 && pageNo != 1) {
                GetUserPage(pageNo - 1);
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            hideLoader();
            if (xhr.responseText == '') {
                $("#errorMsg").text(errorThrown + " " + textStatus);
            } else {
                var jsonResponse = JSON.parse(xhr.responseText);
                $("#errorMsg").text(errorThrown + " : " + jsonResponse.Message);
            }
        }
    });
}

function InsertUserProfile(data) {
    if (validData(data)) {
        showLoader("Saving user profile");
        $('#update').hide();
        data.gender = (data.gender == "Male") ? "M" : "F";
        $.ajax({
            url: URL + "/api/v1/UserProfiles",
            type: "POST",
            data: data,
            success: function (data) {
                populateForm(dummyUser);
                GetUserPage($('#current_page').val());
                $('#update').hide();
                $("#successMsg").html("New user added successfully<br/> User ID: " + data.id);
                orderFormat = '1A';
                hideLoader();
            },
            error: function (xhr, textStatus, errorThrown) {
                hideLoader();
                GetUserPage($('#current_page').val());
                data.gender = (data.gender == "M") ? "Male" : "Female";
                if (xhr.responseText == '') {
                    $("#errorMsg").text(errorThrown + " " + textStatus);
                } else {
                    var jsonResponse = JSON.parse(xhr.responseText);
                    $("#errorMsg").text(errorThrown + " : " + jsonResponse.Message);
                }
            }
        });
    }
}

function UpdateUserProfile(data) {
    if (validData(data)) {
        showLoader("Updating user details");
        data.gender = (data.gender == "Male") ? "M" : "F";
        $.ajax({
            url: URL + "/api/v1/UserProfiles/" + data.id,
            type: "PUT",
            data: data,
            success: function (data) {
                populateForm(dummyUser);
                GetUserPage($('#current_page').val());
                $('#update').hide();
                $("#successMsg").text("User detalis updated successfully");
                hideLoader();
            },
            error: function (xhr, textStatus, errorThrown) {
                data.gender = (data.gender == "M") ? "Male" : "Female";
                GetUserPage($('#current_page').val());
                hideLoader();
                if (xhr.responseText == '') {
                    $("#errorMsg").text(errorThrown + " " + textStatus);
                } else {
                    var jsonResponse = JSON.parse(xhr.responseText);
                    $("#errorMsg").text(errorThrown + " : " + jsonResponse.Message);
                    hideLoader();
                }
            }
        });
    }
}

function DeleteUserProfile(data) {
    resetMsgs();
    if (confirm("Confirm Delete..!!")) {
        showLoader("Deleting user profile");
        $.ajax({
            url: URL + "/api/v1/UserProfiles/" + data.id,
            type: "DELETE",
            success: function () {
                GetUserPage($('#current_page').val());
                $("#successMsg").text("User  " + data.firstName + " " + data.lastName + " Deleted Successfully");
                hideLoader();
            },
            error: function (xhr, textStatus, errorThrown) {
                GetUserPage($('#current_page').val());
                hideLoader();
                if (xhr.responseText == '') {
                    $("#errorMsg").text(errorThrown + " " + textStatus);
                } else {
                    var jsonResponse = JSON.parse(xhr.responseText);
                    $("#errorMsg").text(errorThrown + " : " + jsonResponse.Message);
                    hideLoader();
                }
            }
        });
    }
}

function validData(user) {
    resetMsgs();
    var nameCheck = (validateName(user.firstName) && validateName(user.lastName));
    var dobCheck = validateDOB(user.dateOfBirth);
    var genderCheck = validateGender(user.gender);
    return nameCheck && dobCheck && genderCheck;
}

function validateName(name) {
    if (/^[a-zA-Z]+[a-zA-Z\s]+$/.test(name)) {
        return true;
    } else {
        $("#errorMsg").append("* Name should not contain numbers or special characters<br>");
        return false;
    }
}

function validateDOB(dob) {
    if (!(dob == '')) {
        if (dob < '1900/01/01') {
            $("#errorMsg").append("* User can't be Immortal<br/>&nbsp&nbsp&nbspCheck Date of Birth<br>");
            return false;
        }
        if (dob > '2014/12/31') {
            $("#errorMsg").append("* User is too young to have a user profile<br/>&nbsp&nbsp&nbspCheck Date of Birth<br>");
            return false;
        }
        return true;
    } else {
        $("#errorMsg").append("* Invalid Date of Birth<br>");
        return false;
    }
}

function validateGender(gender) {
    if (gender == 'Male' || gender == 'Female') {
        return true;
    } else {
        $("#errorMsg").append("* Invalid Gender selection<br>");
        return false;
    }
}

var dummyUser = {
    id: 0,
    firstName: "",
    lastName: "",
    dateOfBirth: "",
    gender: ""
};


function viewModel() {
    var self = this;
    self.Users = ko.observableArray();
    self.dataForm = ko.observable({
        id: 0,
        firstName: "",
        lastName: "",
        dateOfBirth: "",
        gender: ""
    });

    // CURD operations
    self.fillForm = populateForm;
    self.updateUser = UpdateUserProfile;
    self.addUser = InsertUserProfile;
    self.deleteUser = DeleteUserProfile;

    // Sorting
    self.orderByColumn = function (data, event) {
        var colNo = event.target.attributes.colNo.value;
        if (orderFormat == colNo + 'A') {
            orderFormat = colNo + 'D';
        } else {
            orderFormat = colNo + 'A';
        }
        GetUserPage($('#current_page').val());
    };
}
ko.applyBindings(viewModel);


function makePager(current_page) {
    $('#current_page').val(current_page);
    numOfPages = Math.ceil(totalUsers / 10);
    var pagerHTML = "";
    for (var i = 1; i <= numOfPages; i++) {
        pagerHTML += "<a class='" + ((i == current_page) ? "currentPageButton" : "numericButton") + "' href=\"javascript:GetUserPage(" + i + ")\" longdesc='" + i + "'>" + (i) + "</a>&nbsp;";
    }
    $('#pager').html(pagerHTML);
    $('#prevButton').css('visibility', 'visible');;
    $('#nextButton').css('visibility', 'visible');;
    if (current_page == 1) {
        $('#prevButton').css('visibility', 'hidden');
    }
    if (current_page == numOfPages) {
        $('#nextButton').css('visibility', 'hidden');
    }

}

function showLoader(message) {
    $("#loader").text(message);
    $("#loader").show();
    $("#save").prop('disabled', true);
    $("#reset").prop('disabled', true);
    $("#update").prop('disabled', true);
}

function hideLoader() {
    $("#loader").hide();
    $("#save").prop('disabled', false);
    $("#reset").prop('disabled', false);
    $("#update").prop('disabled', false);
}

function populateForm(data) {
    var userData = $.extend(true, {}, data);
    self.dataForm(userData);
    resetMsgs();
    hideLoader();
    $('#update').show();
}

function resetForm() {
    populateForm(dummyUser);
    $('#update').hide();
}

function resetMsgs() {
    $("#errorMsg").text("");
    $("#successMsg").text("");
}

function next() {
    resetMsgs();
    var new_page = parseInt($('#current_page').val()) + 1;
    GetUserPage(new_page);
}


function previous() {
    resetMsgs();
    var new_page = parseInt($('#current_page').val()) - 1;
    GetUserPage(new_page);
}

currCol = 0;
function highlightColumn(index) {
    if (index > 0) {
        if (currCol > 0)
            $('#usersTable td:nth-child(' + currCol + ')').removeClass('highlighted ');
        currCol = index;
        $('#usersTable td:nth-child(' + index + ')').addClass('highlighted');
    }
}