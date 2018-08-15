
function CheckPassword()
{
    var val = document.getElementById("NewPassword").value;
    var meter = document.getElementById("meter");
    var no = 0;

    try
    {
        if (val != "")
        {
            if (val.length < 8)
            { no = 1; }

            // 8 Characters With Lower Case Letter, Upper Case Letter, Number Or Special Character
            if (val.length >= 8 &&
                val.match(/[a-zA-Z]/) || val.match(/\d+/) || val.match(/[~`!#$%^&*()-+=\{}\[\]<>\,."\/'?|]/))
            { no = 2; }

            // 8 Characters With Lower Case Letter, Upper Case Letter And Number
            if (val.length >= 8 &&
               (val.match(/[a-z]/) && val.match(/[A-Z]/)) &&
                val.match(/\d+/))
            { no = 3; }

            // 8 Characters With Lower Case Letter, Upper Case Letter, Number, And Special Characters
            if (val.length >= 8 &&
               (val.match(/[a-z]/) && val.match(/[A-Z]/)) &&
                val.match(/\d+/) &&
                val.match(/[~`!#$%^&*()-+=\{}\[\]<>\,."\/'?|]/))
            { no = 4; }

            // 16 Characters With Lower Case Letter, Upper Case Letter, Number, And Special Characters
            if (val.length >= 16 &&
               (val.match(/[a-z]/) && val.match(/[A-Z]/)) &&
                val.match(/\d+/) &&
                val.match(/[~`!#$%^&*()-+=\{}\[\]<>\,."\/'?|]/))
            { no = 5; }

            // 26 Characters With Lower Case Letter, Upper Case Letter, Number, And Special Characters
            if (val.length == 26 &&
               (val.match(/[a-z]/) && val.match(/[A-Z]/)) &&
                val.match(/\d+/) &&
                val.match(/[~`!#$%^&*()-+=\{}\[\]<>\,."\/'?|]/))
            { no = 6; }

            if (no == 1)
            {
                $("#meter").animate({ width: '150px' }, 300);
                meter.style.backgroundColor = "red";
                document.getElementById("pass_type").innerHTML = "Strength: Very Weak";
            }
            if (no == 2)
            {
                $("#meter").animate({ width: '170px' }, 300);
                meter.style.backgroundColor = "#F5BCA9";
                document.getElementById("pass_type").innerHTML = "Strength: Weak";
            }
            if (no == 3)
            {
                $("#meter").animate({ width: '190px' }, 300);
                meter.style.backgroundColor = "#FF8000";
                document.getElementById("pass_type").innerHTML = "Strength: Good";
            }
            if (no == 4)
            {
                $("#meter").animate({ width: '200px' }, 300);
                meter.style.backgroundColor = "#00FF40";
                document.getElementById("pass_type").innerHTML = "Strength: Strong";
            }
            if (no == 5)
            {
                $("#meter").animate({ width: '220px' }, 300);
                meter.style.backgroundColor = "#00FF40";
                document.getElementById("pass_type").innerHTML = "Strength: Very Strong";
            }
            if (no == 6)
            {
                $("#meter").animate({ width: '240px' }, 300);
                meter.style.backgroundColor = "#00FF40";
                document.getElementById("pass_type").innerHTML = "Strength: Strongest";
            }
        }
        else
        {
            meter.style.backgroundColor = "white";
            meter.style.width = "120px";
            document.getElementById("pass_type").innerHTML = "";
        }
    }
    catch (ex)
    {
    }
}
