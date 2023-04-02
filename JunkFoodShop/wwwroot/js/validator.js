function Validator(options) {
    var selectorRules = {};
    function validate(inputElement, rule) {
        var errorMessage;
        var errorElement = inputElement.parentElement.querySelector(
            options.errorSelector
        );

        // Lấy ra các rule của selector
        var rules = selectorRules[rule.selector];

        // Lặp qua từng rule và kiểm tra
        // Nếu có lỗi thì dừng kiểm tra
        for (var i = 0; i < rules.length; i++) {
            errorMessage = rules[i](inputElement.value);
            if (errorMessage) break;
        }
        if (errorMessage) {
            errorElement.innerText = errorMessage;
            inputElement.parentElement.classList.add("invalid");
        } else {
            errorElement.innerText = "";
            inputElement.parentElement.classList.remove("invalid");
        }
        return !errorMessage;
    }

    // Lấy element của form
    var formElement = document.querySelector(options.form);
    if (formElement) {
        formElement.onsubmit = function (e) {
            e.preventDefault();

            var isFormValid = true;

            // Thực hiện lặp qua từng rule và validate
            options.rules.forEach((rule) => {
                var inputElement = formElement.querySelector(rule.selector);
                var isValid = validate(inputElement, rule);
                if (!isValid) {
                    isFormValid = false;
                }
            });

            if (isFormValid) {
                // Trường hợp submit với JS
                if (typeof options.onSubmit === "function") {
                    var formEnableInputs = formElement.querySelectorAll(
                        "[name]:not([disable])"
                    );
                    var formValues = Array.from(formEnableInputs).reduce(
                        function (values, input) {
                            return (values[input.name] = input.value) && values;
                        },
                        {}
                    );
                    options.onSubmit(formValues);
                } else {
                    // Trường hợp submit với hành vi mặc định
                    formElement.submit();
                }
            }
        };

        // Lặp qua mỗi rule và xử lý (lắng nghe sự kiện blur, input, ...)
        options.rules.forEach((rule) => {
            // Lưu lại các rule cho mỗi input
            if (Array.isArray(selectorRules[rule.selector])) {
                selectorRules[rule.selector].push(rule.test);
            } else {
                selectorRules[rule.selector] = [rule.test];
            }

            var inputElement = formElement.querySelector(rule.selector);
            if (inputElement) {
                // Xử lý trường hợp blur input
                inputElement.onblur = function () {
                    validate(inputElement, rule);
                };
                // Xử lý trường hợp mỗi khi người dùng nhập vào input
                inputElement.oninput = function () {
                    var errorElement = inputElement.parentElement.querySelector(
                        options.errorSelector
                    );
                    errorElement.innerText = "";
                    inputElement.parentElement.classList.remove("invalid");
                };
            }
        });
    }
}

// Định nghĩa rule:
// Nguyên tắc các rule:
// 1. Khi có lỗi => trả ra lỗi
// 2. Khi hợp lệ => không trả ra gì (undefined)

Validator.isRequired = function (selector, message) {
    return {
        selector,
        test: function (value) {
            return value.trim()
                ? undefined
                : message ?? "Vui lòng nhập trường này";
        },
    };
};

Validator.isEmail = function (selector, message) {
    return {
        selector,
        test: function (value) {
            var regex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            return regex.test(value)
                ? undefined
                : message ?? "Trường này phải là email";
        },
    };
};

Validator.minLength = function (selector, min, message) {
    return {
        selector,
        test: function (value) {
            return value.length >= min
                ? undefined
                : message ?? `Vui lòng nhập tối thiểu ${min} ký tự`;
        },
    };
};

Validator.isPasswordConfirmed = function (selector, getConfirmValue, message) {
    return {
        selector,
        test: function (value) {
            return value === getConfirmValue()
                ? undefined
                : message ?? "Giá trị nhập vào không chính xác";
        },
    };
};

Validator.isPhoneNumber = function (selector, number, message) {
    return {
        selector,
        test: function (value) {
            return value.length === number
                ? undefined
                : message ?? `Vui lòng nhập đúng ${number} số`;
        },
    };
};

Validator.isImageLink = function (selector, message) {
    return {
        selector,
        test: function (value) {
            var regex = /(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|gif|png)/;
            return regex.test(value)
                ? undefined
                : message ?? "Trường này phải là liên kết hình ảnh";
        },
    };
};
