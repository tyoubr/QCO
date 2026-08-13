// =========================================================
// RECAP INFO - 4 STEP CREATE / EDIT PAGE JS
// =========================================================

$(document).ready(function () {

    // =====================================================
    // SELECT2
    // =====================================================

    if ($.fn.select2) {

        $("#StyleName").select2({
            width: "100%",
            placeholder: "-- Select Style Name --",
            allowClear: true
        });

        $("#TeamLeaderName").select2({
            width: "100%",
            placeholder: "-- Select Team Leader --",
            allowClear: true
        });

        $("#BookingNo").select2({
            width: "100%",
            placeholder: "-- Select Internal Ref. --",
            allowClear: true
        });

        $("#itemNameInput").select2({
            width: "100%",
            placeholder: "-- Select Item Name --",
            allowClear: true
        });
    }


    // =====================================================
    // VARIABLES
    // =====================================================

    let currentStep = 1;

    const totalSteps = 4;


    // =====================================================
    // SHOW STEP
    // =====================================================

    function showStep(step) {

        // ---------------------------------------------
        // Safety
        // ---------------------------------------------

        if (step < 1) {
            step = 1;
        }

        if (step > totalSteps) {
            step = totalSteps;
        }


        currentStep = step;


        // =================================================
        // HIDE ALL STEP CONTENT
        // =================================================

        $(".step-content").hide();


        // =================================================
        // SHOW CURRENT STEP
        // =================================================

        $("#step" + step).show();


        // =================================================
        // RESET STEPPER
        // =================================================

        $(".step-wrapper")
            .removeClass("active completed");

        $(".step-line")
            .removeClass("active");


        // =================================================
        // STEP 1
        // =================================================

        if (step === 1) {

            $("#step1Wrapper")
                .addClass("active");
        }
        else {

            $("#step1Wrapper")
                .addClass("completed");

            $("#stepLine1")
                .addClass("active");
        }


        // =================================================
        // STEP 2
        // =================================================

        if (step === 2) {

            $("#step2Wrapper")
                .addClass("active");
        }
        else if (step > 2) {

            $("#step2Wrapper")
                .addClass("completed");

            $("#stepLine2")
                .addClass("active");
        }


        // =================================================
        // STEP 3
        // =================================================

        if (step === 3) {

            $("#step3Wrapper")
                .addClass("active");
        }
        else if (step > 3) {

            $("#step3Wrapper")
                .addClass("completed");

            $("#stepLine3")
                .addClass("active");
        }


        // =================================================
        // STEP 4
        // =================================================

        if (step === 4) {

            $("#step4Wrapper")
                .addClass("active");
        }


        // =================================================
        // DEBUG
        // =================================================

        console.log(
            "Current Step:",
            currentStep
        );
    }


    // =====================================================
    // INITIAL STEP
    // =====================================================

    showStep(1);


    // =====================================================
    // STYLE -> BUYER
    // =====================================================

    $(document).on(
        "change",
        "#StyleName",
        function () {

            const buyerName =
                $(this)
                    .find("option:selected")
                    .data("buyer") || "";

            $("#BuyerName")
                .val(buyerName);
        }
    );


    // =====================================================
    // ADD ITEM
    // =====================================================

    $("#addItem").on(
        "click",
        function () {

            const itemName =
                $("#itemNameInput").val();

            const offeredQty =
                $("#offeredQtyInput").val();

            const quotedPrice =
                $("#quotedPriceInput").val();

            const remarks =
                $("#remarksInput").val();


            // =================================================
            // CHECKBOX VALUES
            // =================================================

            const isPrint =
                $("#isPrintInput").is(":checked")
                    ? 1
                    : 0;

            const isWash =
                $("#isWashInput").is(":checked")
                    ? 1
                    : 0;

            const isEmb =
                $("#isEmbInput").is(":checked")
                    ? 1
                    : 0;


            // =================================================
            // VALIDATION
            // =================================================

            if (!itemName) {

                alert(
                    "Please select Item Name."
                );

                return;
            }


            if (
                offeredQty === "" ||
                offeredQty === null ||
                offeredQty === undefined
            ) {

                alert(
                    "Please enter Offered Qty."
                );

                return;
            }


            if (
                quotedPrice === "" ||
                quotedPrice === null ||
                quotedPrice === undefined
            ) {

                alert(
                    "Please enter Quoted Price."
                );

                return;
            }


            // =================================================
            // SERIAL
            // =================================================

            const serial =
                $("#itemsBody tr").length + 1;


            // =================================================
            // DISPLAY TEXT
            // =================================================

            const printText =
                isPrint === 1
                    ? "Yes"
                    : "No";

            const washText =
                isWash === 1
                    ? "Yes"
                    : "No";

            const embText =
                isEmb === 1
                    ? "Yes"
                    : "No";


            // =================================================
            // TABLE ROW
            // =================================================

            const row = `
                <tr
                    data-item-name="${escapeHtmlAttribute(itemName)}"
                    data-offered-qty="${escapeHtmlAttribute(offeredQty)}"
                    data-quoted-price="${escapeHtmlAttribute(quotedPrice)}"
                    data-is-print="${isPrint}"
                    data-is-wash="${isWash}"
                    data-is-emb="${isEmb}"
                    data-remarks="${escapeHtmlAttribute(remarks)}"
                >

                    <td class="text-center align-middle">
                        ${serial}
                    </td>

                    <td class="text-center align-middle item-name-cell">
                        ${escapeHtml(itemName)}
                    </td>

                    <td class="text-center align-middle">
                        ${escapeHtml(offeredQty)}
                    </td>

                    <td class="text-center align-middle">
                        ${escapeHtml(quotedPrice)}
                    </td>

                    <td class="text-center align-middle">
                        ${printText}
                    </td>

                    <td class="text-center align-middle">
                        ${washText}
                    </td>

                    <td class="text-center align-middle">
                        ${embText}
                    </td>

                    <td class="text-center align-middle remarks-cell">
                        ${escapeHtml(remarks)}
                    </td>

                    <td class="text-center align-middle">

                        <button
                            type="button"
                            class="btn btn-sm btn-danger delete-item">

                            Remove

                        </button>

                    </td>

                </tr>
            `;


            // =================================================
            // ADD ROW
            // =================================================

            $("#itemsBody")
                .append(row);


            // =================================================
            // CLEAR INPUTS
            // =================================================

            $("#itemNameInput")
                .val("")
                .trigger("change");

            $("#offeredQtyInput")
                .val("");

            $("#quotedPriceInput")
                .val("");

            $("#remarksInput")
                .val("");

            $("#isPrintInput")
                .prop("checked", false);

            $("#isWashInput")
                .prop("checked", false);

            $("#isEmbInput")
                .prop("checked", false);


            // =================================================
            // HIDDEN INPUTS
            // =================================================

            buildItemHiddenInputs();


            // =================================================
            // RENUMBER
            // =================================================

            renumberItems();
        }
    );


    // =====================================================
    // DELETE ITEM
    // =====================================================

    $(document).on(
        "click",
        ".delete-item",
        function () {

            $(this)
                .closest("tr")
                .remove();

            renumberItems();

            buildItemHiddenInputs();
        }
    );


    // =====================================================
    // RENUMBER ITEMS
    // =====================================================

    function renumberItems() {

        $("#itemsBody tr")
            .each(
                function (index) {

                    $(this)
                        .find("td:first")
                        .text(index + 1);
                }
            );
    }


    // =====================================================
    // BUILD ITEM HIDDEN INPUTS
    // =====================================================

    function buildItemHiddenInputs() {

        const $container =
            $("#itemDetailsInputs");


        if (!$container.length) {

            console.error(
                "#itemDetailsInputs not found."
            );

            return;
        }


        $container.empty();


        $("#itemsBody tr")
            .each(
                function (index) {

                    const $row =
                        $(this);


                    const itemName =
                        $row.attr("data-item-name") || "";

                    const offeredQty =
                        $row.attr("data-offered-qty") || "";

                    const quotedPrice =
                        $row.attr("data-quoted-price") || "";

                    const remarks =
                        $row.attr("data-remarks") || "";

                    const isPrint =
                        parseInt(
                            $row.attr("data-is-print") || "0",
                            10
                        );

                    const isWash =
                        parseInt(
                            $row.attr("data-is-wash") || "0",
                            10
                        );

                    const isEmb =
                        parseInt(
                            $row.attr("data-is-emb") || "0",
                            10
                        );


                    // =================================================
                    // ITEM NAME
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].ItemName`,
                        itemName
                    );


                    // =================================================
                    // OFFERED QTY
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].OfferedQty`,
                        offeredQty
                    );


                    // =================================================
                    // QUOTED PRICE
                    // IMPORTANT:
                    // Model property = QuoatedPrice
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].QuoatedPrice`,
                        quotedPrice
                    );


                    // =================================================
                    // REMARKS
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].Remarks`,
                        remarks
                    );


                    // =================================================
                    // PRINT
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsPrint`,
                        isPrint
                    );


                    // =================================================
                    // WASH
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsWash`,
                        isWash
                    );


                    // =================================================
                    // EMBROIDERY
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsEmb`,
                        isEmb
                    );
                }
            );
    }


    // =====================================================
    // APPEND HIDDEN INPUT
    // =====================================================

    function appendHiddenInput(
        $container,
        name,
        value
    ) {

        $("<input>", {

            type: "hidden",

            name: name,

            value:
                value === null ||
                    value === undefined
                    ? ""
                    : value

        })
            .appendTo($container);
    }


    // =====================================================
    // STEP 1 -> STEP 2
    // =====================================================

    $("#nextStep1").on(
        "click",
        function () {

            const styleName =
                $("#StyleName").val();


            // ---------------------------------------------
            // STYLE VALIDATION
            // ---------------------------------------------

            if (!styleName) {

                alert(
                    "Please select Style Name."
                );

                return;
            }


            // ---------------------------------------------
            // ITEM VALIDATION
            // ---------------------------------------------

            const itemCount =
                $("#itemsBody tr").length;


            if (itemCount === 0) {

                alert(
                    "Please add at least one Item."
                );

                return;
            }


            // ---------------------------------------------
            // REBUILD
            // ---------------------------------------------

            buildItemHiddenInputs();


            // ---------------------------------------------
            // STEP 2
            // ---------------------------------------------

            showStep(2);
        }
    );


    // =====================================================
    // BOOKING / INTERNAL REF CHANGE
    // =====================================================

    $("#BookingNo").on(
        "change",
        function () {

            const bookingNo =
                $(this).val();


            // =================================================
            // CLEAR OLD DATA
            // =================================================

            $("#PoNo")
                .val("");

            $("#detailsBody")
                .empty();

            $("#recapDetailsInputs")
                .empty();

            $("#noDetailsMessage")
                .hide();


            // =================================================
            // NO BOOKING
            // =================================================

            if (!bookingNo) {

                console.log(
                    "Internal Ref cleared."
                );

                return;
            }


            // =================================================
            // LOADING
            // =================================================

            $("#detailsBody")
                .html(`

                    <tr>

                        <td colspan="9"
                            class="text-center">

                            Loading Recap Details...

                        </td>

                    </tr>

                `);


            $("#BookingNo")
                .prop("disabled", true);


            // =================================================
            // AJAX
            // =================================================

            $.ajax({

                url:
                    "/RecapeInfo/GetDetailsByBookingNo",

                type:
                    "GET",

                data:
                {
                    bookingNo:
                        bookingNo
                },

                dataType:
                    "json",


                // =================================================
                // SUCCESS
                // =================================================

                success:
                    function (response) {

                        console.log(
                            "GetDetailsByBookingNo response:",
                            response
                        );


                        $("#detailsBody")
                            .empty();

                        $("#recapDetailsInputs")
                            .empty();

                        $("#noDetailsMessage")
                            .hide();


                        // -----------------------------------------
                        // RESPONSE VALIDATION
                        // -----------------------------------------

                        if (!response) {

                            alert(
                                "Failed to load Recap Details."
                            );

                            return;
                        }


                        if (
                            response.success === false
                        ) {

                            alert(
                                response.message ||
                                "Failed to load Recap Details."
                            );

                            return;
                        }


                        // -----------------------------------------
                        // PO NUMBER
                        // -----------------------------------------

                        $("#PoNo")
                            .val(
                                response.poNo || ""
                            );


                        // -----------------------------------------
                        // DATA
                        // -----------------------------------------

                        const data =
                            Array.isArray(response.data)
                                ? response.data
                                : [];


                        // -----------------------------------------
                        // NO DATA
                        // -----------------------------------------

                        if (data.length === 0) {

                            $("#noDetailsMessage")
                                .show();


                            $("#detailsBody")
                                .html(`

                                    <tr>

                                        <td colspan="9"
                                            class="text-center text-muted">

                                            No recap details found for this Internal Ref.

                                        </td>

                                    </tr>

                                `);

                            return;
                        }


                        // -----------------------------------------
                        // BUILD TABLE
                        // -----------------------------------------

                        data.forEach(
                            function (item, index) {

                                const itemName =
                                    item.itemName ?? "";

                                const bodyPart =
                                    item.bodyPart ?? "";

                                const fabrication =
                                    item.fabrication ?? "";

                                const gsm =
                                    item.gsm ?? "";

                                const color =
                                    item.color ?? "";

                                const consPcs =
                                    item.consPcs ?? "";

                                const totalQty =
                                    item.totalQty ?? "";


                                const row = `

                                    <tr

                                        data-item-name="${escapeHtmlAttribute(itemName)}"

                                        data-body-part="${escapeHtmlAttribute(bodyPart)}"

                                        data-fabrication="${escapeHtmlAttribute(fabrication)}"

                                        data-gsm="${escapeHtmlAttribute(gsm)}"

                                        data-color="${escapeHtmlAttribute(color)}"

                                        data-cons-pcs="${escapeHtmlAttribute(consPcs)}"

                                        data-total-qty="${escapeHtmlAttribute(totalQty)}"

                                    >

                                        <td class="text-center align-middle">
                                            ${index + 1}
                                        </td>

                                        <td class="text-center align-middle">
                                            ${escapeHtml(itemName)}
                                        </td>

                                        <td class="text-center align-middle">
                                            ${escapeHtml(bodyPart)}
                                        </td>

                                        <td class="text-center align-middle">
                                            ${escapeHtml(fabrication)}
                                        </td>

                                        <td class="text-center align-middle">
                                            ${escapeHtml(gsm)}
                                        </td>

                                        <td class="text-center align-middle">
                                            ${escapeHtml(color)}
                                        </td>

                                        <td class="text-center align-middle">
                                            ${escapeHtml(consPcs)}
                                        </td>

                                        <td class="text-center align-middle">
                                            ${escapeHtml(totalQty)}
                                        </td>

                                        <td class="text-center align-middle">

                                            <button
                                                type="button"
                                                class="btn btn-sm btn-danger delete-recap-detail">

                                                Remove

                                            </button>

                                        </td>

                                    </tr>

                                `;


                                $("#detailsBody")
                                    .append(row);
                            }
                        );


                        // -----------------------------------------
                        // HIDDEN INPUTS
                        // -----------------------------------------

                        buildRecapDetailHiddenInputsFromTable();
                    },


                // =================================================
                // ERROR
                // =================================================

                error:
                    function (xhr) {

                        console.error(
                            "GetDetailsByBookingNo ERROR:",
                            xhr
                        );

                        console.error(
                            "Response:",
                            xhr.responseText
                        );


                        $("#detailsBody")
                            .empty();

                        $("#recapDetailsInputs")
                            .empty();

                        $("#noDetailsMessage")
                            .hide();


                        alert(
                            "Failed to load Recap Details."
                        );
                    },


                // =================================================
                // COMPLETE
                // =================================================

                complete:
                    function () {

                        $("#BookingNo")
                            .prop(
                                "disabled",
                                false
                            );
                    }

            });

        }
    );


    // =====================================================
    // DELETE RECAP DETAIL
    // =====================================================

    $(document).on(
        "click",
        ".delete-recap-detail",
        function () {

            $(this)
                .closest("tr")
                .remove();


            renumberRecapDetails();


            buildRecapDetailHiddenInputsFromTable();


            const remainingRows =
                $("#detailsBody tr")
                    .filter(function () {

                        return $(this)
                            .find(".delete-recap-detail")
                            .length > 0;

                    })
                    .length;


            if (remainingRows === 0) {

                $("#noDetailsMessage")
                    .show();


                $("#detailsBody")
                    .html(`

                        <tr>

                            <td colspan="9"
                                class="text-center text-muted">

                                No Recap Details available.

                            </td>

                        </tr>

                    `);


                $("#recapDetailsInputs")
                    .empty();
            }
        }
    );


    // =====================================================
    // RENUMBER RECAP DETAILS
    // =====================================================

    function renumberRecapDetails() {

        $("#detailsBody tr")
            .each(
                function (index) {

                    $(this)
                        .find("td:first")
                        .text(index + 1);
                }
            );
    }


    // =====================================================
    // BUILD RECAP DETAIL HIDDEN INPUTS
    // =====================================================

    function buildRecapDetailHiddenInputsFromTable() {

        const $container =
            $("#recapDetailsInputs");


        if (!$container.length) {

            console.error(
                "#recapDetailsInputs not found."
            );

            return;
        }


        $container.empty();


        $("#detailsBody tr")
            .each(
                function (index) {

                    const $row =
                        $(this);


                    // -----------------------------------------
                    // Ignore message row
                    // -----------------------------------------

                    if (
                        !$row.find(
                            ".delete-recap-detail"
                        ).length
                    ) {

                        return;
                    }


                    const itemName =
                        $row.attr("data-item-name") || "";

                    const bodyPart =
                        $row.attr("data-body-part") || "";

                    const fabrication =
                        $row.attr("data-fabrication") || "";

                    const gsm =
                        $row.attr("data-gsm") || "";

                    const color =
                        $row.attr("data-color") || "";

                    const consPcs =
                        $row.attr("data-cons-pcs") || "";

                    const totalQty =
                        $row.attr("data-total-qty") || "";


                    // =================================================
                    // MODEL FIELDS
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `Details[${index}].ItemName`,
                        itemName
                    );

                    appendHiddenInput(
                        $container,
                        `Details[${index}].BodyPart`,
                        bodyPart
                    );

                    appendHiddenInput(
                        $container,
                        `Details[${index}].Fabrication`,
                        fabrication
                    );

                    appendHiddenInput(
                        $container,
                        `Details[${index}].Gsm`,
                        gsm
                    );

                    appendHiddenInput(
                        $container,
                        `Details[${index}].ColorName`,
                        color
                    );

                    appendHiddenInput(
                        $container,
                        `Details[${index}].ConsPerUnit`,
                        consPcs
                    );

                    appendHiddenInput(
                        $container,
                        `Details[${index}].TotalQty`,
                        totalQty
                    );
                }
            );
    }


    // =====================================================
    // OLD FUNCTION SUPPORT
    // =====================================================

    function buildRecapDetailHiddenInputs(data) {

        const $container =
            $("#recapDetailsInputs");


        if (!$container.length) {

            console.error(
                "#recapDetailsInputs not found."
            );

            return;
        }


        $container.empty();


        if (
            !Array.isArray(data) ||
            data.length === 0
        ) {

            return;
        }


        data.forEach(
            function (item, index) {

                const itemName =
                    item.itemName ?? "";

                const bodyPart =
                    item.bodyPart ?? "";

                const fabrication =
                    item.fabrication ?? "";

                const gsm =
                    item.gsm ?? "";

                const color =
                    item.color ?? "";

                const consPcs =
                    item.consPcs ?? "";

                const totalQty =
                    item.totalQty ?? "";


                appendHiddenInput(
                    $container,
                    `Details[${index}].ItemName`,
                    itemName
                );

                appendHiddenInput(
                    $container,
                    `Details[${index}].BodyPart`,
                    bodyPart
                );

                appendHiddenInput(
                    $container,
                    `Details[${index}].Fabrication`,
                    fabrication
                );

                appendHiddenInput(
                    $container,
                    `Details[${index}].Gsm`,
                    gsm
                );

                appendHiddenInput(
                    $container,
                    `Details[${index}].ColorName`,
                    color
                );

                appendHiddenInput(
                    $container,
                    `Details[${index}].ConsPerUnit`,
                    consPcs
                );

                appendHiddenInput(
                    $container,
                    `Details[${index}].TotalQty`,
                    totalQty
                );
            }
        );
    }


    // =====================================================
    // STEP 2 -> STEP 1
    // =====================================================

    $("#previousStep2").on(
        "click",
        function () {

            showStep(1);
        }
    );


    // =====================================================
    // STEP 2 -> STEP 3
    // =====================================================

    $("#nextStep2").on(
        "click",
        function () {

            const bookingNo =
                $("#BookingNo").val();


            // =================================================
            // OPTIONAL INTERNAL REF
            // =================================================

            if (!bookingNo) {

                console.log(
                    "No Internal Ref selected."
                );

                showStep(3);

                return;
            }


            // =================================================
            // REBUILD DETAILS
            // =================================================

            buildRecapDetailHiddenInputsFromTable();


            // =================================================
            // COUNT REAL DETAILS
            // =================================================

            const detailCount =
                $("#detailsBody tr")
                    .filter(function () {

                        return $(this)
                            .find(".delete-recap-detail")
                            .length > 0;

                    })
                    .length;


            if (detailCount === 0) {

                alert(
                    "No Recap Detail found for the selected Internal Ref."
                );

                return;
            }


            // =================================================
            // STEP 3
            // =================================================

            showStep(3);
        }
    );


    // =====================================================
    // STEP 3 -> STEP 2
    // =====================================================

    $("#previousStep3").on(
        "click",
        function () {

            showStep(2);
        }
    );


    // =====================================================
    // STEP 3 -> STEP 4
    // =====================================================

    $(document).on(
        "click",
        "#nextStep3",
        function () {

            console.log(
                "STEP 3 -> STEP 4"
            );


            // =================================================
            // OPTIONAL FACTORY VALIDATION
            // =================================================

            // If you want factory selection required,
            // uncomment this section.
            //
            // const sewingFactory =
            //     $("#SewingFactory").val();
            //
            // if (!sewingFactory) {
            //
            //     alert("Please select Sewing Factory.");
            //     return;
            // }


            // =================================================
            // GO STEP 4
            // =================================================

            showStep(4);
        }
    );


    // =====================================================
    // STEP 4 -> STEP 3
    // =====================================================

    $(document).on(
        "click",
        "#previousStep4",
        function () {

            console.log(
                "STEP 4 -> STEP 3"
            );

            showStep(3);
        }
    );


    // =====================================================
    // FORM SUBMIT
    // =====================================================

    $("#recapForm").on(
        "submit",
        function (e) {

            console.log(
                "========================================"
            );

            console.log(
                "========== FINAL SUBMIT =========="
            );

            console.log(
                "========================================"
            );


            // =================================================
            // REBUILD ITEM DETAILS
            // =================================================

            buildItemHiddenInputs();


            // =================================================
            // REBUILD RECAP DETAILS
            // =================================================

            const bookingNo =
                $("#BookingNo").val();


            if (bookingNo) {

                buildRecapDetailHiddenInputsFromTable();

            }
            else {

                $("#recapDetailsInputs")
                    .empty();
            }


            // =================================================
            // ITEM DETAILS COUNT
            // =================================================

            const itemCount =
                $("#itemDetailsInputs")
                    .find(
                        "input[name^='ItemDetails['][name$='.ItemName']"
                    )
                    .length;


            console.log(
                "Final ItemDetails count:",
                itemCount
            );


            // =================================================
            // RECAP DETAILS COUNT
            // =================================================

            const detailCount =
                $("#recapDetailsInputs")
                    .find(
                        "input[name^='Details['][name$='.ItemName']"
                    )
                    .length;


            console.log(
                "Final Details count:",
                detailCount
            );


            // =================================================
            // ITEM VALIDATION
            // =================================================

            if (itemCount === 0) {

                e.preventDefault();


                alert(
                    "Please add at least one Item."
                );


                showStep(1);


                return false;
            }


            // =================================================
            // DETAIL VALIDATION
            // =================================================

            if (
                bookingNo &&
                detailCount === 0
            ) {

                e.preventDefault();


                alert(
                    "No Recap Detail found for the selected Internal Ref."
                );


                showStep(2);


                return false;
            }


            // =================================================
            // FINAL DEBUG
            // =================================================

            console.log(
                "Master.StyleName:",
                $("#StyleName").val()
            );

            console.log(
                "Master.BookingNo:",
                $("#BookingNo").val()
            );

            console.log(
                "Master.PoNo:",
                $("#PoNo").val()
            );


            console.log(
                "----- ITEM DETAILS -----"
            );


            $("#itemDetailsInputs input")
                .each(
                    function () {

                        console.log(
                            $(this).attr("name"),
                            "=",
                            $(this).val()
                        );
                    }
                );


            console.log(
                "----- RECAP DETAILS -----"
            );


            $("#recapDetailsInputs input")
                .each(
                    function () {

                        console.log(
                            $(this).attr("name"),
                            "=",
                            $(this).val()
                        );
                    }
                );


            // =================================================
            // FINAL FORM DATA
            // =================================================

            const formData =
                new FormData(this);


            console.log(
                "----- FINAL FORM DATA -----"
            );


            for (
                const pair of formData.entries()
            ) {

                console.log(
                    pair[0],
                    "=",
                    pair[1]
                );
            }


            console.log(
                "Submitting form..."
            );


            return true;
        }
    );


    // =====================================================
    // HTML ESCAPE
    // =====================================================

    function escapeHtml(value) {

        if (
            value === null ||
            value === undefined
        ) {

            return "";
        }


        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }


    // =====================================================
    // HTML ATTRIBUTE ESCAPE
    // =====================================================

    function escapeHtmlAttribute(value) {

        if (
            value === null ||
            value === undefined
        ) {

            return "";
        }


        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/"/g, "&quot;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");
    }


    // =====================================================
    // EXISTING ITEM ROWS
    // =====================================================

    if (
        $("#itemsBody tr").length > 0
    ) {

        renumberItems();

        buildItemHiddenInputs();
    }


    // =====================================================
    // EXISTING RECAP DETAIL ROWS
    // =====================================================

    if (
        $("#detailsBody tr").length > 0
    ) {

        renumberRecapDetails();

        buildRecapDetailHiddenInputsFromTable();
    }


    // =====================================================
    // FINAL
    // =====================================================

    console.log(
        "Recap Info 4-Step JS loaded successfully."
    );

});