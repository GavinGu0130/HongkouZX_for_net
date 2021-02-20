/*tony维护*/
function getChecked(obj, txt) {
    var str = '';
    $(obj + ' input:checkbox:checked').each(function () {
        if (str != '') str += txt;
        str += $(this).val();
    });
    return str;
}
function checkCheck(obj) {
    var err = '';
    var txt = ($(obj).attr('title')) ? $(obj).attr('title') : '';
    var str = getChecked(obj, '');
    if (str == '') {
        err = '请选择[' + txt + ']';
    }
    if (err) {
        alert(err);
        $(obj + ' input:checkbox:first').focus();
        return true;
    }
}
function checkSelect(obj) {
    var err = '';
    var txt = ($(obj).attr('title')) ? $(obj).attr('title') : '';
    var str = $(obj).val();
    if (str == '') {
        err = '请选择[' + txt + ']';
    }
    if (err) {
        alert(err);
        $(obj).focus();
        return true;
    }
}
function checkRadio(obj) {
    var err = '';
    var txt = ($(obj).attr('title')) ? $(obj).attr('title') : '';
    var str = $(obj + ' input:radio:checked').val();
    if (!str) {
        err = '请选择[' + txt + ']';
    }
    if (err) {
        alert(err);
        $(obj + ' input:radio:first').focus();
        return true;
    }
}
function checkEmpty(obj, min, max, mod) {
    var err = '';
    var txt = ($(obj).attr('title')) ? $(obj).attr('title') : '';
    var str = $(obj).val().replace(/^\s+|\s+$/g, '');
    $(obj).val(str);
    if (str == '' || str == txt) {
        err = '请填写[' + txt + ']';
    } else if (min > 0 && str.length < min) {
        err = '[' + txt + ']太短了，必须>=' + min + '个字符';
    } else if (max > 0 && str.length > max) {
        err = '[' + txt + ']太长了，必须<=' + max + '个字符';
    } else if (mod) {
        switch (mod) {
            case 3:
                var reg = /^(?![a-zA-z]+$)(?!\d+$)(?![!@#$%^&*]+$)(?![a-zA-z\d]+$)(?![a-zA-z!@#$%^&*]+$)(?![\d!@#$%^&*]+$)[a-zA-Z\d!@#$%^&*]+$/;
                if (!reg.test(str)) {
                    err = txt + '必须包括[字母、数字、特殊字符]';
                }
                break;
            case 2:
                var reg = /^(?![a-zA-z]+$)(?!\d+$)[a-zA-Z\d*]+$/;
                if (!reg.test(str)) {
                    err = txt + '必须包括[字母、数字]';
                }
                break;
            case 1:
                if (str.indexOf('123456') == 0 || str.lastIndexOf('654321') == str.length - 6) {
                    err = '[' + txt + ']设置过于简单了'
                } else {
                    var s = '';
                    var n = 0;
                    for (i = 0; i < str.length; i++) {
                        var tmp = str.substr(i, 1);
                        if (s.indexOf(tmp) < 0) {
                            s += tmp;
                            n++;
                        }
                    }
                    if (n <= 1) {
                        err = '[' + txt + ']不能设置为相同字符'
                    }
                }
                break;
            default:
                break;
        }
    }
    if (err) {
        alert(err);
        $(obj).focus();
        return true;
    }
}
function checkEmail(obj) {
    var err = '';
    var txt = ($(obj).attr('title')) ? $(obj).attr('title') : '';
    var str = $(obj).val().replace(/^\s+|\s+$/g, '');
    $(obj).val(str);
    var reg = /^([0-9A-Za-z\-_\.]+)@([0-9a-z]+\.[a-z]{2,3}(\.[a-z]{2})?)$/g;
    if (str == '' || str == txt) {
        err = '请填写[' + txt + ']';
    } else if (!reg.test(str)) {
        err = '请填写正确的[' + txt + ']';
    }
    if (err) {
        alert(err);
        $(obj).focus();
        return true;
    }
}
function formatMobile(obj) {
    var code = ($(obj).attr('type') == 'text') ? $(obj).val() : $(obj).text();
    code = code.replace(/\s+/g, '');
    var str = code.substr(0, 3);
    for (i = 3; i < code.length; i += 4) {
        str += ' ' + code.substr(i, 4);
    }
    if ($(obj).attr('type') == 'text') {
        $(obj).val(str);
    } else {
        $(obj).text(str);
    }
}
function checkMobile(obj) {
    var err = '';
    var txt = ($(obj).attr('title')) ? $(obj).attr('title') : '';
    var str = $(obj).val().replace(/^\s+|\s+$/g, '');
    str = str.replace(/[\s|\-|—|a-zA-Z|\u4E00-\u9FA5]+/g, '');
    $(obj).val(str);
    formatMobile(obj);
    var reg = /^(\+86|86)?([1][3|4|5|6|7|8|9]\d{9})$/g;
    if (str == '' || str == txt) {
        err = '请填写[' + txt + ']';
    } else if (str.length != 11) {
        err = '请填写11位[' + txt + ']';
    } else if (!reg.test(str)) {
        err = '请填写正确的[' + txt + ']';
    }
    if (err) {
        alert(err);
        $(obj).focus();
        return true;
    }
}
function checkDate(obj) {
    var txt = ($(obj).attr('title')) ? $(obj).attr('title') : '';
    var str = $(obj).val().replace(/^\s+|\s+$/g, '');
    $(obj).val(str);
    var err = checkDate2(str);
    if (err) {
        alert(err);
        $(obj).focus();
        return true;
    }
}
function checkDate2(str, txt) {
    if (!txt) txt = '日期';
    if (str == '') {
        return '请填写[' + txt + ']';
    }
    var reg = /^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29))$/;
    if (!reg.test(str)) {
        return '请填写正确的[' + txt + ']';
    }
    return '';
}
function formatCard(obj) {
    var reg = /^[A-Z]$/;
    var code = ($(obj).attr('type') == 'text') ? $(obj).val() : $(obj).text();
    code = code.replace(/\s+/g, '').toUpperCase().replace(/（/g, '(').replace(/）/g, ')');//
    var str = code.substr(0, 1);
    if (reg.test(str)) {//护照
        if (code.indexOf('(') > 0) {
            str = code.substring(0, code.indexOf('(')) + ' ' + code.substring(code.indexOf('('));
        } else {
            for (i = 1; i < code.length; i += 4) {
                str += ' ' + code.substr(i, 4);
            }
        }
    } else {//身份证、台胞证
        str = code.substr(0, 3);
        if (code.length > 3) {
            str += ' ' + code.substr(3, 3);
        }
        for (i = 6; i < code.length; i += 4) {
            str += ' ' + code.substr(i, 4);
        }
    }
    if ($(obj).attr('type') == 'text') {
        $(obj).val(str);
    } else {
        $(obj).text(str);
    }
}
//1、将前面的身份证号码17位数分别乘以不同的系数。从第一位到第十七位的系数分别为：7－9－10－5－8－4－2－1－6－3－7－9－10－5－8－4－2。
//2、将这17位数字和系数相乘的结果相加。
//3、用加出来和除以11，看余数是多少？
//4、余数只可能有0－1－2－3－4－5－6－7－8－9－10这11个数字。其分别对应的最后一位身份证的号码为1－0－X －9－8－7－6－5－4－3－2。
function checkCardCode(obj, objDate, objSex) {
    var err = '';
    var txt = ($(obj).attr('title')) ? $(obj).attr('title') : '';
    var code = $(obj).val().replace(/[\[\]\s|\/|\-|—|a-z|\u4E00-\u9FA5]+/g, '');
    if (txt) {
        code = code.replace(txt, '');
    }
    $(obj).val(code);
    formatCard(obj);
    if (code.length >= 15) {//身份证
        var reg = /^(\d{17}[0-9Xx])|(\d{15})$/g;
        if (!reg.test(code)) {
            err = '请填写正确的[' + txt + ']号码';
        } else {
            var sex = -1;
            var birthday = '';
            if (code.length == 18) {
                sex = code.substr(16, 1);
                birthday = code.substr(6, 4) + '-' + code.substr(10, 2) + '-' + code.substr(12, 2);
                var arr = new Array();
                for (var i = 0; i < code.length; i++) {
                    arr[i] = code.substr(i, 1);
                }
                var test = (arr[0] * 7 + arr[1] * 9 + arr[2] * 10 + arr[3] * 5 + arr[4] * 8 + arr[5] * 4 + arr[6] * 2 + arr[7] * 1 + arr[8] * 6 + arr[9] * 3 + arr[10] * 7 + arr[11] * 9 + arr[12] * 10 + arr[13] * 5 + arr[14] * 8 + arr[15] * 4 + arr[16] * 2) % 11;
                if ((test == 0 && arr[17] == 1) || (test == 1 && arr[17] == 0) || (test == 2 && arr[17] == 'X') || (test == 3 && arr[17] == 9) || (test == 4 && arr[17] == 8) || (test == 5 && arr[17] == 7) || (test == 6 && arr[17] == 6) || (test == 7 && arr[17] == 5) || (test == 8 && arr[17] == 4) || (test == 9 && arr[17] == 3) || (test == 10 && arr[17] == 2)) {
                } else {
                    err = '请填写正确的[' + txt + ']号码';
                }
            } else if (code.length == 15) {
                sex = code.substr(14, 1);
                birthday = '19' + code.substr(6, 2) + '-' + code.substr(8, 2) + '-' + code.substr(10, 2);
            } else {
                err = '请填写18位[' + txt + ']号码';
            }
            if (err == '') {
                err = checkDate2(birthday, '出生日期');
            }
            if (err == '') {
                if (objDate) {
                    $(objDate).val(birthday);
                }
                if (objSex) {
                    if (parseInt(sex, 10) % 2 == 0) {
                        $('input[name="' + objSex + '"]:last').click();
                    } else {
                        $('input[name="' + objSex + '"]:first').click();
                    }
                }
            }
        }
    } else {
        var reg = /^([A-Z][0-9]{8})|([A-Z]{2}[0-9]{7})|([0-9]{8,14})$/g;
        if (!reg.test(code)) {
            err = '请填写正确的[' + txt + ']号码';
        }
    }
    if (err) {
        alert(err);
        $(obj).focus();
        return true;
    }
}
