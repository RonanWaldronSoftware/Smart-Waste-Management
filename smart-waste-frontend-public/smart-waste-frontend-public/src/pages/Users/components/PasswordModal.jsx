import { useFormik } from "formik";
import React from "react";
import { useTranslation } from "react-i18next";
import { useSelector } from "react-redux";
import { toast } from "react-toastify";
import * as yup from "yup";
import LoaderSmall from "../../../components/LoaderSmall";
import PasswordInput from "../../../components/PasswordInput";

const PasswordModal = ({ modalType, handleModal, handleFormSubmit }) => {
  const { t } = useTranslation("translation");

  const passwordSchema = yup.object().shape({
    oldPassword:
      modalType !== "adminReset"
        ? yup.string().required(t("fieldRequired")).min(5, t("passError"))
        : yup.string(),
    newPassword: yup
      .string()
      .required(t("fieldRequired"))
      .min(5, t("passError")),
    confirmPassword: yup
      .string()
      .required(t("fieldRequired"))
      .min(5, t("passError")),
  });
  const formik = useFormik({
    initialValues: {
      oldPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
    validationSchema: passwordSchema,
    onSubmit: async (values, { setSubmitting }) => {
      if (values.newPassword !== values.confirmPassword) {
        toast.error(t("passMatch"));
        setSubmitting(false);
        return;
      }
      try {
        await handleFormSubmit(values);
      } finally {
        setSubmitting(false);
      }
    },
  });

  const isExpanded = useSelector((state) => state.nav.isExpanded);

  return (
    <div
      className="fixed top-0 left-0 w-screen h-screen grid place-items-center p-4"
      style={{ paddingLeft: isExpanded ? "240px" : "80px", zIndex: 99 }}
    >
      {/* Overlay */}
      <div
        className="absolute top-0 left-0 w-full h-full bg-black bg-opacity-50 z-30"
        onClick={() => handleModal("")}
      ></div>

      {/* Modal */}
      <div
        className="bg-white flex flex-col p-6 rounded z-50 gap-8 w-full h-full max-w-[1100px] max-h-[750px] overflow-auto"
        style={{ boxShadow: "0 2px 2px rgba(0,0,0,.25)" }}
      >
        {/* Modal header */}
        <div className="flex items-center justify-between">
          <h3 className="font-medium text-[1.75rem]">{t("changePassword")}</h3>

          <button
            className=" text-2xl font-bold p-4 -my-4 -mr-4 ml-auto"
            onClick={() => handleModal("")}
          >
            x
          </button>
        </div>

        {/* Modal body */}
        <div className="">
          <form onSubmit={formik.handleSubmit}>
            <div className="flex flex-col gap-6">
              {modalType !== "adminReset" && (
                <PasswordInput
                  id={"oldPassword"}
                  name={"oldPassword"}
                  value={formik.values.oldPassword}
                  onChange={formik.handleChange}
                  onBlur={formik.handleBlur}
                  error={
                    formik.errors.oldPassword && formik.touched.oldPassword
                      ? formik.errors.oldPassword
                      : null
                  }
                  label={t("oldPassword")}
                />
              )}
              <PasswordInput
                id={"newPassword"}
                name={"newPassword"}
                value={formik.values.newPassword}
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                error={
                  formik.errors.newPassword && formik.touched.newPassword
                    ? formik.errors.newPassword
                    : null
                }
                label={t("newPassword")}
              />
              <PasswordInput
                id={"confirmPassword"}
                name={"confirmPassword"}
                value={formik.values.confirmPassword}
                onChange={formik.handleChange}
                onBlur={formik.handleBlur}
                error={
                  formik.errors.confirmPassword &&
                  formik.touched.confirmPassword
                    ? formik.errors.confirmPassword
                    : null
                }
                label={t("confirmPassword")}
              />

              {/* Separator */}
              <span className="block col-span-1 md:col-span-2 lg:col-span-3 border-t border-solid border-themeColors-bgLightColor"></span>

              <div className="flex  col-span-1 md:col-span-2 lg:col-span-3 items-center justify-end gap-3">
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={() => handleModal("")}
                >
                  {t("back")}
                </button>
                <button
                  type="submit"
                  className="btn btn-primary min-h-[38px]"
                  disabled={formik.isSubmitting}
                  style={{ opacity: formik.isSubmitting ? 0.5 : 1 }}
                >
                  {formik.isSubmitting ? <LoaderSmall /> : t("save")}
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default PasswordModal;
