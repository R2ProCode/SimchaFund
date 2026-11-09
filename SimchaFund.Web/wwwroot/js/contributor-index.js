$(function () {
    $("#new-contributor").on("click", function () {
        const form = $(".new-contributor form");

        $("#initialDepositDiv").show();
        form[0].reset();

        new bootstrap.Modal($(".new-contributor")[0]).show();
    });

    $(".deposit-button").on("click", function () {
        const contributorId = $(this).data("id");
        const tr = $(this).closest("tr");
        const name = tr.find("td:eq(1)").text();
                
        $('.deposit [name="contributorId"]').val(contributorId);
        $("#deposit-name").text(name);

        new bootstrap.Modal($(".deposit")[0]).show();
    });

    $("#search").on("keyup", function () {
        const text = $(this).val().toLowerCase();
        $("table tbody tr").each(function () {
            const row = $(this);
            const name = row.find("td:eq(1)").text().toLowerCase();
            row.toggle(name.indexOf(text) >= 0);
        });
    });

    $("#clear").on("click", function () {
        $("#search").val("");
        $("table tbody tr").show();
    });

    const editModal = new bootstrap.Modal($(".edit-modal")[0]);

    $(".edit-contributor").on("click", function () {
        const form = $(".edit-contributor form");
        const id = $(this).data("id");
        const firstName = $(this).data("firstName");
        const lastName = $(this).data("lastName");
        const cell = $(this).data("cell");
        const alwaysInclude = $(this).data("alwaysInclude");

        console.log("What jQuery sees:", $(this).data());

        $("#edit_id").val(id);
        $("#edit-contributor_first_name").val(firstName);
        $("#edit-contributor_last_name").val(lastName);
        $("#edit-contributor_cell_number").val(cell);
        $("#edit-contributor_always_include").prop("checked", alwaysInclude === true || alwaysInclude === "True" );

        editModal.show();
    });
});
