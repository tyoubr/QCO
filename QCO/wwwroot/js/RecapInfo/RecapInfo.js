// =========================================================
// RECAP INFO - CREATE PAGE JS
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


    // =====================================================
    // SHOW STEP
    // =====================================================

    function showStep(step) {

        currentStep = step;

        $("#step1").hide();
        $("#step2").hide();
        $("#step3").hide();

        $("#step" + step).show();


        // =================================================
        // STEPPER UI
        // =================================================

        $(".step-wrapper")
            .removeClass("active completed");

        $(".step-line")
            .removeClass("active");


        // =================================================
        // STEP 1
        // =================================================

        if (step >= 1) {

            $("#step1Wrapper").addClass(
                step === 1
                    ? "active"
                    : "completed"
            );
        }


        // =================================================
        // STEP 2
        // =================================================

        if (step >= 2) {

            $("#stepLine1")
                .addClass("active");

            $("#step2Wrapper").addClass(
                step === 2
                    ? "active"
                    : "completed"
            );
        }


        // =================================================
        // STEP 3
        // =================================================

        if (step >= 3) {

            $("#stepLine2")
                .addClass("active");

            $("#step3Wrapper").addClass(
                step === 3
                    ? "active"
                    : "completed"
            );
        }
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


            // =================================================
            // MODEL:
            // TblRecapItemDetails
            //
            // IsPrint
            // IsWash
            // IsEmb
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
            // DISPLAY
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
                >

                    <td>
                        ${serial}
                    </td>

                    <td>
                        ${escapeHtml(itemName)}
                    </td>

                    <td>
                        ${escapeHtml(offeredQty)}
                    </td>

                    <td>
                        ${escapeHtml(quotedPrice)}
                    </td>

                    <td>
                        ${printText}
                    </td>

                    <td>
                        ${washText}
                    </td>

                    <td>
                        ${embText}
                    </td>

                    <td>

                        <button
                            type="button"
                            class="btn btn-sm btn-danger delete-item">

                            Remove

                        </button>

                    </td>

                </tr>
            `;


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

            $("#isPrintInput")
                .prop(
                    "checked",
                    false
                );

            $("#isWashInput")
                .prop(
                    "checked",
                    false
                );

            $("#isEmbInput")
                .prop(
                    "checked",
                    false
                );


            // =================================================
            // BUILD HIDDEN INPUTS
            // =================================================

            buildItemHiddenInputs();


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


        // -------------------------------------------------
        // REMOVE OLD INPUTS
        // -------------------------------------------------

        $container.empty();


        // -------------------------------------------------
        // LOOP ITEM ROWS
        // -------------------------------------------------

        $("#itemsBody tr")
            .each(
                function (index) {

                    const $row =
                        $(this);


                    // =================================================
                    // GET DATA FROM HTML DATA ATTRIBUTES
                    // =================================================

                    const itemName =
                        $row.attr("data-item-name") || "";

                    const offeredQty =
                        $row.attr("data-offered-qty") || "";

                    const quotedPrice =
                        $row.attr("data-quoted-price") || "";

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
                    // MODEL MATCH
                    // =================================================

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].ItemName`,
                        itemName
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].OfferedQty`,
                        offeredQty
                    );


                    // IMPORTANT:
                    // Model property is QuoatedPrice
                    // NOT QuotedPrice

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].QuoatedPrice`,
                        quotedPrice
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsPrint`,
                        isPrint
                    );


                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsWash`,
                        isWash
                    );


                    // IMPORTANT:
                    // Model property is IsEmb

                    appendHiddenInput(
                        $container,
                        `ItemDetails[${index}].IsEmb`,
                        isEmb
                    );
                }
            );


        // =================================================
        // DEBUG
        // =================================================

        console.log(
            "========== ITEM DETAILS =========="
        );

        console.log(
            "ItemDetails rows:",
            $("#itemsBody tr").length
        );

        console.log(
            "ItemDetails hidden inputs:",
            $container.find(
                "input[name^='ItemDetails[']"
            ).length
        );

        console.log(
            "ItemDetails HTML:",
            $container.html()
        );
    }


    // =====================================================
    // HELPER - APPEND HIDDEN INPUT
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

            // ---------------------------------------------
            // Validate Style
            // ---------------------------------------------

            const styleName =
                $("#StyleName").val();


            if (!styleName) {

                alert(
                    "Please select Style Name."
                );

                return;
            }


            // ---------------------------------------------
            // Validate Item
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
            // Rebuild before next step
            // ---------------------------------------------

            buildItemHiddenInputs();


            // ---------------------------------------------
            // Go Step 2
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


            // ---------------------------------------------
            // Clear old data
            // ---------------------------------------------

            $("#PoNo")
                .val("");

            $("#detailsBody")
                .empty();

            $("#recapDetailsInputs")
                .empty();

            $("#noDetailsMessage")
                .hide();


            // ---------------------------------------------
            // No booking selected
            //
            // IMPORTANT:
            // Internal Ref is OPTIONAL.
            // ---------------------------------------------

            if (!bookingNo) {

                console.log(
                    "Internal Ref cleared. Recap Details are optional."
                );

                return;
            }


            // ---------------------------------------------
            // Loading
            // ---------------------------------------------

            $("#detailsBody").html(`

                <tr>

                    <td colspan="8"
                        class="text-center">

                        Loading Recap Details...

                    </td>

                </tr>

            `);


            $("#BookingNo")
                .prop(
                    "disabled",
                    true
                );


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


                        // -----------------------------------------
                        // Clear
                        // -----------------------------------------

                        $("#detailsBody")
                            .empty();

                        $("#recapDetailsInputs")
                            .empty();

                        $("#noDetailsMessage")
                            .hide();


                        // -----------------------------------------
                        // Response validation
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
                            Array.isArray(
                                response.data
                            )
                                ? response.data
                                : [];


                        console.log(
                            "Recap Detail Count:",
                            data.length
                        );


                        // -----------------------------------------
                        // NO DATA
                        // -----------------------------------------

                        if (
                            data.length === 0
                        ) {

                            $("#noDetailsMessage")
                                .show();


                            $("#detailsBody")
                                .html(`

                                    <tr>

                                        <td colspan="8"
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
                            function (
                                item,
                                index
                            ) {

                                // =================================
                                // CONTROLLER RESPONSE
                                // =================================

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

                                    <tr>

                                        <td>
                                            ${index + 1}
                                        </td>

                                        <td>
                                            ${escapeHtml(
                                    itemName
                                )}
                                        </td>

                                        <td>
                                            ${escapeHtml(
                                    bodyPart
                                )}
                                        </td>

                                        <td>
                                            ${escapeHtml(
                                    fabrication
                                )}
                                        </td>

                                        <td>
                                            ${escapeHtml(
                                    gsm
                                )}
                                        </td>

                                        <td>
                                            ${escapeHtml(
                                    color
                                )}
                                        </td>

                                        <td>
                                            ${escapeHtml(
                                    consPcs
                                )}
                                        </td>

                                        <td>
                                            ${escapeHtml(
                                    totalQty
                                )}
                                        </td>

                                    </tr>

                                `;


                                $("#detailsBody")
                                    .append(row);
                            }
                        );


                        // -----------------------------------------
                        // Generate hidden inputs
                        // -----------------------------------------

                        buildRecapDetailHiddenInputs(
                            data
                        );
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
    // BUILD RECAP DETAIL HIDDEN INPUTS
    // =====================================================

    function buildRecapDetailHiddenInputs(
        data
    ) {

        const $container =
            $("#recapDetailsInputs");


        if (!$container.length) {

            console.error(
                "#recapDetailsInputs not found."
            );

            return;
        }


        // ---------------------------------------------
        // CLEAR OLD INPUTS
        // ---------------------------------------------

        $container.empty();


        if (
            !Array.isArray(data) ||
            data.length === 0
        ) {

            console.warn(
                "No recap details to generate."
            );

            return;
        }


        // ---------------------------------------------
        // Generate Details[index]
        // ---------------------------------------------

        data.forEach(
            function (
                item,
                index
            ) {

                // =========================================
                // CONTROLLER RESPONSE
                // =========================================

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


                // =========================================
                // MODEL:
                // TblRecapDetails
                // =========================================

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


                // IMPORTANT:
                // Model property is ColorName

                appendHiddenInput(
                    $container,
                    `Details[${index}].ColorName`,
                    color
                );


                // IMPORTANT:
                // Model property is ConsPerUnit

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


        // =================================================
        // DEBUG
        // =================================================

        const detailCount =
            $container
                .find(
                    "input[name^='Details[']"
                )
                .length;


        console.log(
            "========== RECAP DETAILS =========="
        );

        console.log(
            "Generated Details input count:",
            detailCount
        );

        console.log(
            "Generated Details row count:",
            data.length
        );

        console.log(
            "Details HTML:",
            $container.html()
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
            // INTERNAL REF IS OPTIONAL
            // =================================================
            //
            // If Internal Ref is EMPTY:
            //     No warning
            //     No detail validation
            //     Go directly to Step 3
            //
            // If Internal Ref EXISTS:
            //     Details must exist
            //
            // =================================================


            // ---------------------------------------------
            // NO INTERNAL REF
            // ---------------------------------------------

            if (!bookingNo) {

                console.log(
                    "No Internal Ref selected. Skipping Recap Detail validation."
                );

                showStep(3);

                return;
            }


            // ---------------------------------------------
            // INTERNAL REF EXISTS
            // ---------------------------------------------

            const detailCount =
                $("#recapDetailsInputs")
                    .find(
                        "input[name^='Details['][name$='.ItemName']"
                    )
                    .length;


            console.log(
                "Details count before Step 3:",
                detailCount
            );


            // ---------------------------------------------
            // VALIDATE DETAILS
            // ONLY WHEN INTERNAL REF EXISTS
            // ---------------------------------------------

            if (detailCount === 0) {

                alert(
                    "No Recap Detail found for the selected Internal Ref."
                );

                return;
            }


            // ---------------------------------------------
            // GO STEP 3
            // ---------------------------------------------

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
            // RECAP DETAILS
            // =================================================
            //
            // Normally generated during BookingNo change.
            //
            // If BookingNo is empty:
            //     Details can remain empty.
            //
            // =================================================


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
            // VALIDATE ITEM DETAILS
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
            // INTERNAL REF
            // =================================================

            const bookingNo =
                $("#BookingNo").val();


            // =================================================
            // VALIDATE RECAP DETAILS
            //
            // ONLY IF INTERNAL REF EXISTS
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
            // DEBUG MASTER
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


            // =================================================
            // DEBUG ITEM DETAILS
            // =================================================

            console.log(
                "----- ITEM DETAILS FIELDS -----"
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


            // =================================================
            // DEBUG RECAP DETAILS
            // =================================================

            console.log(
                "----- RECAP DETAILS FIELDS -----"
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
            // FINAL FORM DATA DEBUG
            // =================================================

            const formData =
                new FormData(
                    this
                );


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
            .replace(
                /&/g,
                "&amp;"
            )
            .replace(
                /</g,
                "&lt;"
            )
            .replace(
                />/g,
                "&gt;"
            )
            .replace(
                /"/g,
                "&quot;"
            )
            .replace(
                /'/g,
                "&#039;"
            );
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
            .replace(
                /&/g,
                "&amp;"
            )
            .replace(
                /"/g,
                "&quot;"
            )
            .replace(
                /</g,
                "&lt;"
            )
            .replace(
                />/g,
                "&gt;"
            );
    }


    // =====================================================
    // OPTIONAL:
    // HANDLE EXISTING ITEM ROWS ON PAGE LOAD
    // =====================================================

    if (
        $("#itemsBody tr").length > 0
    ) {

        renumberItems();

        buildItemHiddenInputs();
    }

});