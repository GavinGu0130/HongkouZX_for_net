/*tony维护*/
var upFileUrl = '../cn/upload.aspx';
function upFile(txt, obj, btn, type) {
    if (txt) {
        if (obj) {
            $(txt).change(function () {
                var url = $(this).val();
                if (url) {
                    switch (type) {
                        case 'img':
                        case 'photo':
                            $(obj).html('<img src="' + url + '" />');
                            break;
                        case 'video':
                            //<iframe src="' + url + '" width="' + w + '" height="' + h + '" frameborder="0" scrolling="no"></iframe>
                            //<embed type="application/x-shockwave-flash" src="' + url + '" wmode="opaque" quality="high" bgcolor="#ffffff" menu="false" play="true" loop="true" width="' + w + '" height="' + h + '"/>
                            //<embed type="application/x-mplayer2" src="'+url+'" enablecontextmenu="false" autostart="'+(play=='1'?'true':'false')+'" width="'+w+'" height="'+h+'"/>
                            //$(obj).html('<embed type="application/x-shockwave-flash" src="' + url + '" wmode="opaque" quality="high" bgcolor="#ffffff" menu="false" play="true" loop="true" width="300" height="200" />');
                            //$(obj).html('<iframe src="' + url + '" width="300" height="200" frameborder="0" scrolling="no"></iframe>');
                            $(obj).html('<embed type="application/x-mplayer2" src="' + url + '" enablecontextmenu="false" autostart="false" width="300" height="200"/>');
                            break;
                        case 'doc':
                        case 'file':
                            $(obj).on('click', 'div>u', function () {
                                var $parent = $(this).parent();
                                $(txt).val($(txt).val().replace($parent.find('a').text(), '').replace(/\|\|/g, '|'));
                                $parent.remove();
                            });
                            var str = '';
                            var arr = url.split('|');
                            for (i = 0; i < arr.length; i++) {
                                if (arr[i]) {
                                    str += '<div><u>X</u><a href="' + arr[i] + '" target="_blank">' + arr[i] + '</a></div>';
                                }
                            }
                            $(obj).html(str);
                            break;
                        default:
                            $(obj).html('&nbsp;');
                            break;
                    }
                } else {
                    //$(obj).html('<br/>');
                }
            }).change();
        }
    } else {
        txt = '';
    }
    if (btn) {
        if (ie >= 6 && ie < 10) {
            KindEditor.ready(function (K) {
                var editor = K.editor({
                    uploadJson: upFileUrl + '?type=imgs&editor=kind'
                });
                K(btn).click(function () {
                    editor.loadPlugin('image', function () {
                        editor.plugin.imageDialog({
                            //imageUrl: txt,
                            clickFn: function (url, title, width, height, border, align) {
                                url = url.replace("/upload/", "../upload/");
                                $(txt).val(url).change();
                                editor.hideDialog();
                            }
                        });
                    });
                });
            });
            $(btn).click(function () {
                return false;
            });
        } else {
            $(btn).click(function () {
                bindFile();
                return false;
            });
        }
    }
    function bindFile() {
        var upId = $(btn).attr('id') + '_up' + ~(new Date()).getTime();
        // multiple="multiple"
        var accept = '';
        switch (type) {
            case 'img':
            case 'photo':
                accept = 'image/jpg,image/jpeg,image/png,image/gif,image/bmp';
                break;
            case 'video':
                accept = '.mp4,video/*';//,application/*
                break;
            case 'xls':
                accept = '.csv,.xls,xlsx,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet';//,application/msexcel
                break;
            case 'doc':
                accept = '.doc,.docx,application/msword';//,application/vnd.ms-word
                break;
            case 'xml':
                accept = 'text/xml,application/xml';
                break;
            default:
                break;
        }
        var upUrl = upFileUrl;
        if (type) {
            upUrl += (upUrl.indexOf('?') >= 0) ? '&' : '?';
            upUrl += 'type=' + type;
        }
        $(btn).after('<input id="' + upId + '" name="' + upId + '" type="file" accept="' + accept + '" style="display:none;" />');
        $('#' + upId).change(function () {
            try {
                //alert($(this)[0]); return;
                var $f = $(this)[0].files;
                //$(btn).val($f.length + '-' + $f[0].name + '-' + $f[0].size + '-' + $f[0].type);
                var ext = ($f[0].name.substr($f[0].name.lastIndexOf("."))).toLowerCase();
                switch (type) {
                    case 'img':
                        if (!(ext == '.jpg' || ext == '.gif' || ext == '.png' || ext == '.jpeg')) {
                            alert('只能上传(.jpg|.jpeg|.gif|.png)文件！');
                            return;
                        }
                        break;
                    default:
                        if ($f[0].size > 50 * 1024 * 1024) {
                            alert('上传文件不能大于50M');
                            return;
                        }
                        break;
                }
                if (type == 'img' && !(ext == '.jpg' || ext == '.gif' || ext == '.png' || ext == '.jpeg')) {
                    alert('只能上传(.jpg|.jpeg|.gif|.png)文件！');
                    return;
                } else if (type == 'xls' && !(ext == '.xls' || ext == 'xlsx')) {
                    alert('只能上传(.xls|.xlsx)文件！');
                    return;
                } else if (type == 'doc' && !(ext == '.doc' || ext == '.docx')) {
                    alert('只能上传(.doc|.docx)文件！');
                    return;
                } else if ($f[0].size > 50 * 1024 * 1024) {
                    alert('上传文件不能大于50M');
                    return;
                }
                $.ajaxFileUpload({
                    url: upUrl, //你处理上传文件的服务端'../cn/upload.ashx'
                    type: 'post',
                    secureuri: false, //一般设置为false
                    fileElementId: upId,//与页面处理代码中file相对应的ID值
                    dataType: 'json', //返回数据类型：xml、script、json、html、text
                    success: function (data, status) {
                        //alert(data);
                        if (data.errno == '0') {
                            if (txt) {
                                var str = '';
                                switch (type) {
                                    //case 'doc':
                                    case 'file':
                                        str = $(txt).val();
                                        if (str) {
                                            str += "|";
                                        }
                                        str += data.data[0];
                                        break;
                                    default:
                                        str = data.data[0];
                                        break;
                                }
                                $(txt).val(str).change();
                            } else if (obj) {
                                var str = '';
                                switch (type) {
                                    case 'img':
                                    case 'photo':
                                        str = $(obj).val() + '[img]' + data.data[0] + '[/img]';
                                        break;
                                    case 'video':
                                        str = $(obj).val() + '[media]' + data.data[0] + '[/media]';
                                        break;
                                    default:
                                        break;
                                }
                                $(obj).val(str);
                            }
                        } else {
                            alert(data.msg);
                        }
                    },
                    //提交失败自动执行的处理函数
                    error: function (data, status, e) {
                        alert(e);
                    },
                    complete: function () {
                        $('#' + upId).remove();
                    }
                });
            } catch (err) {
                //alert(err);
                return false;
            }
        }).click();
    }
}
function showDialog(title, url, body, w, h, scr) {
    if (!w) w = 400;
    if (!h) h = 300;
    if (url) {
        if (!scr) {
            scr = 'auto';
        }
        body = '<iframe src="' + url + '" width="100%" height="100%" frameborder="0" scrolling="' + scr + '"></iframe>';
    }
    else if (!body) {
        //body = '1234';
    }
    var h2 = (title) ? h - 36 : h;
    var str = '<div id="dialog"><div style="width:' + w + 'px;height:' + h + 'px;margin-left:-' + w / 2 + 'px;margin-top:-' + h / 2 + 'px;"><div class="title">' + title + '<a href="#">X</a></div><div class="body" style="height:' + h2 + 'px;">' + body + '</div></div></div>';
    $('body').append(str);
    $('#dialog>div>.title>a').click(function () {
        $('#dialog').remove();
        return false;
    });
}
function loadSelect(obj, sel, sel2, txt, hf) {
    var arr = eval($(obj).val());
    $(sel).change(function () {
        var str = (txt) ? '<option value="">' + txt + '</option>' : '';
        if ($(sel).val() != '') {
            for (i = 0; i < arr.length; i++) {
                if ($(sel).val() == arr[i][0]) {
                    for (j = 1; j < arr[i].length; j++) {
                        if (arr[i][j]) {
                            str += '<option value="' + arr[i][j] + '">' + arr[i][j] + '</option>';
                        }
                    }
                }
            }
        }
        $(sel2).html(str).change();
    }).change();
    if (hf) {
        $(sel2).change(function () {
            $(hf).val($(this).val());
        });
        if ($(hf).val()) {
            $(sel2 + '>option').each(function () {
                if ($(this).attr('value') == $(hf).val()) {
                    $(this).attr('selected', 'selected');
                }
            });
        } else {
            $(sel2).change();
        }
    }
}
function loadSelMenu(obj, sel, sel2, client, add) {
    var arr = eval($(obj).val());
    var str = '';
    var str = '';
    for (i = 0; i < arr.length; i++) {
        str += '<span title="' + arr[i][0] + '">' + arr[i][0];
        var str2 = '';
        for (j = 1; j < arr[i].length; j++) {
            str2 += '<a>' + arr[i][j] + '</a>';
        }
        if (str2) {
            str += '<div>' + str2 + '</div>';
        }
        str += '</span>';
    }
    if (str) {
        $(sel2).html('<div>' + str + '</div>');
    }
    $(sel2).on('click', 'div>span', function () {
        if (!$(this).find('div').text()) {
            var str = '';
            if (add) {
                str = $(sel).val();
                if (str) {
                    str += ',';
                }
            }
            $(sel).val(str + $(this).attr('title')).change();
            $(sel2).hide();
        }
    }).on('mouseover', 'div>span', function () {
        $(this).addClass('cur').find('div').show();
    }).on('mouseout', 'div>span', function () {
        $(this).removeClass('cur').find('div').hide();
    }).on('click', 'div>span>div>a', function () {
        var str = '';
        if (add) {
            str = $(sel).val();
            if (str) {
                str += ',';
            }
        }
        $(sel).val(str + $(this).parent().parent().attr('title') + '-' + $(this).text()).change();
        $(sel2).hide();
    });
    if (client == 'm') {
        $(sel).click(function () {
            if ($(this).hasClass('cur')) {
                $(this).removeClass('cur');
                $(sel2).hide();
            } else {
                $(this).addClass('cur');
                $(sel2).show();
            }
        });
    } else {
        $(sel).focus(function () {
            if ($(sel2).text()) {
                $(sel2).show();
            }
        }).blur(function () {
            setTimeout(function () {
                $(sel2).hide();
            }, 300);
        });
    }
}
function loadEditor(txt, obj, type) {
    try {
        if (ie >= 6 && ie < 10) {
            KindEditor.ready(function (K) {
                var editor = K.create(txt, {
                    //fileManagerJson: '../inc/kindeditor/asp.net/file_manager_json.ashx', allowFileManager: true,
                    uploadJson: upFileUrl + '?type=imgs&editor=kind',//'../inc/kindeditor/asp.net/upload_json.ashx'
                    pasteType: 1, resizeType: 1, allowPreviewEmoticons: false,
                    items : ['bold', 'italic', 'underline', 'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist', 'insertunorderedlist']//'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', , '|', 'emoticons', 'image', 'link'
                });
            });
        } else {
            var $code = $(txt);
            var E = window.wangEditor;
            var editor = new E(obj);
            editor.customConfig.uploadImgMaxLength = 5;//限制一次最多上传 5 张图片
            editor.customConfig.uploadImgMaxSize = 10 * 1024 * 1024;//将图片大小限制为 10M
            //editor.customConfig.showLinkImg = false;//网络图片
            //editor.customConfig.debug = true;//打开debug模式
            //editor.customConfig.uploadImgShowBase64 = true;
            editor.customConfig.uploadImgServer = upFileUrl + '?type=imgs';//'upload.ashx'
            editor.customConfig.onchange = function (html) {
                $code.val(html2ubb(html));// 监控变化，同步更新到 textarea
            }
            editor.customConfig.onblur = function (html) {
                $code.val(html2ubb(html));// 监控变化，同步更新到 textarea
            }
            //if ($('meta[name=viewport]').attr('content').indexOf('width=device-width') >= 0) {
            if (!type) {
                editor.customConfig.menus = [
                    'bold', 'italic', 'underline', 'list', 'justify'//'fontSize', 'fontName', , 'head', 'quote', 'table', 'undo', 'redo', 'emoticon', 'image'
                ]
            }
            //}
            editor.create();
            editor.txt.html('<p>' + ubb2html($code.val()) + '</p>');
            $code.change(function () {
                editor.txt.html(ubb2html($(this).val()));
                //loadImg(1);
            }).hide();
            //var $editor = $('#editor');
            //$('#btnFormat').click(function () {
            //    var txt = ubb2text($code.val());
            //    $editor.val(txt).change();
            //    //alert(txt);
            //    //alert($editor.html());
            //});
        }
    } catch (err) {
    }
}
