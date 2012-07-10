$(document).ready(function() {
    var row_counter = 1;
    var title_row = '<tr><td><input class="input typeahead" name="title" type="text" data-provide="typeahead"></td><td><select name="rating"><option>10</option><option>2</option><option>3</option><option>4</option><option>5</option></select></td><td><button class="btn btn-danger button-del"><i class="icon-white icon-minus"></i></button></td></tr>';
    var typeahead_handler = {
        updater : function(item) {
            return item + '!';
        },
        source : ["Alabama","Alaska","Arizona","Arkansas","California","Colorado","Connecticut","Delaware","Florida","Georgia","Hawaii","Idaho","Illinois","Indiana","Iowa","Kansas","Kentucky","Louisiana","Maine","Maryland","Massachusetts","Michigan","Minnesota","Mississippi","Missouri","Montana","Nebraska","Nevada","New Hampshire","New Jersey","New Mexico","New York","North Dakota","North Carolina","Ohio","Oklahoma","Oregon","Pennsylvania","Rhode Island","South Carolina","South Dakota","Tennessee","Texas","Utah","Vermont","Virginia","Washington","West Virginia","Wisconsin","Wyoming"]
    };
    var del_handler = function(event) {
        $(this).unbind('click').parents('tr').remove();
        row_counter--;
        if(row_counter === 1) {
            $('#title-list .button-del').addClass('disabled').attr('disabled', 'disabled');
        }
        event.preventDefault();
    };
    $('#title-list tbody').append(title_row);
    $('#title-list .button-del').addClass('disabled').attr('disabled', 'disabled');
    $('.typeahead').typeahead(typeahead_handler);
    $('.typeahead').on('change', function () {
        console.log($(this).val());
    });
    $('.button-add').click(function(event) {
        if(row_counter === 1) {
            $('#title-list .button-del').removeClass('disabled').removeAttr('disabled');
        }
        var title_body = $('#title-list tbody');
        var test = title_body.append(title_row);
        title_body.find('tr:last .typeahead').typeahead(typeahead_handler);
        title_body.find('tr:last .button-del').click(del_handler);
        row_counter++;
        event.preventDefault();
    });
    $('.button-del').click(del_handler);
});