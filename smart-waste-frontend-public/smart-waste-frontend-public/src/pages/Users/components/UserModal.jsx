import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  createUser,
  getPossibleParents,
  updateUser,
} from "../../../services/users";
import { useTranslation } from "react-i18next";
import * as yup from "yup";
import { useFormik } from "formik";
import { Loader } from "../../../components";
import { fetchWithRetry } from "../../../utils/refreshAccessToken";
import { toast } from "react-toastify";
import LoaderSmall from "../../../components/LoaderSmall";
import PasswordInput from "../../../components/PasswordInput";

const UserModal = ({ modalType, handleModal, modalFormData, fetchData }) => {
  const accessToken = localStorage.getItem("smartWasteAccessToken");
  const configData = useSelector((state) => state.user.configData);
  const currentUser = useSelector((state) => state.user.userData);
  const navigate = useNavigate();
  const { t } = useTranslation("translation");
  const isExpanded = useSelector((state) => state.nav.isExpanded);
  const [loading, setLoading] = useState(true);
  const [formSelectOptions, setFormSelectOptions] = useState({
    countries: configData.countries,
    locales: configData.languages,
    roles: configData.roles,
    possibleParents: [],
  });

  const [parentInactive, setParentInactive] = useState(false);

  const userSchema = yup.object().shape({
    address1: yup.string(),
    address2: yup.string(),
    city: yup.string(),
    binLevelAlert: yup.string(),
    countryId: yup.string().required(t("fieldRequired")),
    email: yup.string().email("Invalid format").required(t("fieldRequired")),
    languageId: yup.string().required(t("fieldRequired")),
    phoneNumber: yup.string(),
    name: yup.string().required(t("fieldRequired")),
    parentId:
      modalFormData.id !== currentUser.id
        ? yup.string().required(t("fieldRequired"))
        : yup.string(),
    roleId: yup.string().required(t("fieldRequired")),
    password:
      modalType === "create"
        ? yup.string().required(t("fieldRequired"))
        : yup.string(),
  });
  const formik = useFormik({
    initialValues: modalFormData,
    validationSchema: userSchema,
    onSubmit: (values) => {
      handleFormSubmit(values);
    },
  });

  useEffect(() => {
    if (configData) {
      setFormSelectOptions((prev) => ({
        ...prev,
        countries: configData?.countries || [],
        locales: configData?.languages || [],
        roles: configData?.roles || [],
      }));
      if (modalType === "create") {
        formik.setFieldValue("roleId", formSelectOptions.roles[0].id);
        formik.setFieldValue("languageId", formSelectOptions.locales[0].id);
        formik.setFieldValue("countryId", formSelectOptions.countries[0].id);
      }
    }
  }, [configData]);

  //fetch select options whenever parentId changes
  useEffect(() => {
    if (formik.values.roleId && formik.values.roleId !== "") {
      fetchPossibleParents();
    }
  }, [formik.values.roleId]);

  const fetchPossibleParents = async () => {
    try {
      const data = await fetchWithRetry(
        getPossibleParents,
        accessToken,
        navigate,
        t,
        1,
        formik.values.roleId
      );

      if (data.status === 200) {
        setFormSelectOptions((prev) => ({
          ...prev,
          possibleParents: data.data ? data.data : [],
        }));
        if (modalType === "create") {
          formik.setFieldValue("parentId", data.data[0].id);
        } else {
          const currentParent = data.data.find(
            (parent) => parent.id === modalFormData.parentId
          );
          if (!currentParent) {
            setParentInactive(true);
          }
        }
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  //handle user create/edit
  const handleFormSubmit = (values) => {
    if (modalType === "create") {
      //validate email
      createNewUser(values);
    } else {
      saveChanges(values);
    }
  };

  const createNewUser = async (data) => {
    try {
      const res = await fetchWithRetry(
        createUser,
        accessToken,
        navigate,
        t,
        data
      );

      if (res.status === 200) {
        toast.success(t("userSuccessfullyCreated"));
        handleModal("");
        fetchData();
      } else {
        formik.setSubmitting(false);
      }
    } catch (error) {
      console.error(error);
      toast.error("requestError");
      formik.setSubmitting(false);
    }
  };
  const saveChanges = async (data) => {
    const apiData = {
      ...data,
      parentId: modalFormData.id !== currentUser.id ? data.parentId : null,
    };
    try {
      const res = await fetchWithRetry(
        updateUser,
        accessToken,
        navigate,
        t,
        apiData
      );

      if (res.status === 200) {
        toast.success(t("userSuccessfullyUpdated"));
        handleModal("");
        fetchData();
      } else {
        formik.setSubmitting(false);
      }
    } catch (error) {
      console.error(error);
      toast.error("requestError");
      formik.setSubmitting(false);
    }
  };

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
        className="bg-white flex flex-col p-6 rounded z-50 gap-8 w-full h-full max-w-[1000px] max-h-[750px] overflow-auto"
        style={{ boxShadow: "0 2px 2px rgba(0,0,0,.25)" }}
      >
        {/* Modal header */}
        <div className="flex items-center justify-between">
          <h3 className="font-medium text-[1.75rem]">
            {modalType === "edit" ? t("editUser") : t("createNewUser")}
          </h3>

          <button
            className=" text-2xl font-bold p-4 -my-4 -mr-4 ml-auto"
            onClick={() => handleModal("")}
          >
            x
          </button>
        </div>

        {/* Modal body */}
        <div className="flex flex-col items-center justify-center">
          {loading ? (
            <Loader />
          ) : (
            <form onSubmit={formik.handleSubmit}>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <div className="flex flex-col gap-2">
                  <label htmlFor="name">{t("name")}</label>
                  <input
                    type="text"
                    id="name"
                    name="name"
                    className={`input ${
                      formik.errors.name && formik.touched.name
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.name}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                  />
                  {formik.errors.name && formik.touched.name ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.name}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>
                <div className="flex flex-col gap-2">
                  <label htmlFor="email">{t("email")}</label>
                  <input
                    type="text"
                    id="email"
                    name="email"
                    className={`input ${
                      formik.errors.email && formik.touched.email
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.email}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                  />
                  {formik.errors.email && formik.touched.email ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.email}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>
                {modalType !== "edit" && (
                  <PasswordInput
                    id={"password"}
                    name={"password"}
                    value={formik.values.password}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                    error={
                      formik.errors.password && formik.touched.password
                        ? formik.errors.password
                        : null
                    }
                    label={t("password")}
                  />
                )}
                {/* <div className="flex flex-col gap-2">
                <label htmlFor="location">Name</label>
                <input
                  type="text"
                  id="location"
                  name="location"
                  placeholder="Location"
                  className="input"
                />
                {formErrors.locationErrorMsg && (
              <p>{formErrors.locationErrorMsg}</p>
            )} 
              </div>*/}
                {modalFormData.id !== currentUser.id && (
                  <div className="flex flex-col gap-2">
                    <label htmlFor="roleId">{t("role")}</label>
                    <select
                      name="roleId"
                      id="roleId"
                      className={`input ${
                        formik.errors.roleId && formik.touched.roleId
                          ? "!border-themeColors-dangerColor"
                          : ""
                      }`}
                      value={formik.values.roleId}
                      onChange={(e) =>
                        formik.setFieldValue("roleId", Number(e.target.value))
                      }
                      onBlur={formik.handleBlur}
                    >
                      {formSelectOptions.roles.map((role) => (
                        <option key={role.id} value={role.id}>
                          {role.name}
                        </option>
                      ))}
                    </select>
                    {formik.errors.roleId && formik.touched.roleId ? (
                      <p className="text-xs text-themeColors-dangerColor">
                        {formik.errors.roleId}
                      </p>
                    ) : (
                      <></>
                    )}
                  </div>
                )}
                {modalFormData.id !== currentUser.id && (
                  <div className="flex flex-col gap-2">
                    <label htmlFor="parentId">{t("parent")}</label>
                    <select
                      name="parentId"
                      id="parentId"
                      className={`input ${
                        formik.errors.parentId && formik.touched.parentId
                          ? "!border-themeColors-dangerColor"
                          : ""
                      }`}
                      value={formik.values.parentId}
                      onChange={(e) => {
                        setParentInactive(false);
                        formik.setFieldValue(
                          "parentId",
                          Number(e.target.value)
                        );
                      }}
                      onBlur={formik.handleBlur}
                    >
                      {modalType !== "create" && (
                        <option value={""}>{t("selectParent")}</option>
                      )}
                      {formSelectOptions.possibleParents.map((item) => (
                        <option key={item.id} value={item.id}>
                          {item.name}
                        </option>
                      ))}
                    </select>
                    {parentInactive && (
                      <p className="text-xs text-themeColors-dangerColor">
                        {t("parentInactive")}
                      </p>
                    )}
                    {formik.errors.parentId && formik.touched.parentId ? (
                      <p className="text-xs text-themeColors-dangerColor">
                        {formik.errors.parentId}
                      </p>
                    ) : (
                      <></>
                    )}
                  </div>
                )}
                <div className="flex flex-col gap-2">
                  <label htmlFor="languageId">{t("language")}</label>
                  <select
                    name="languageId"
                    id="languageId"
                    className={`input ${
                      formik.errors.languageId && formik.touched.languageId
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.languageId}
                    onChange={(e) =>
                      formik.setFieldValue("languageId", Number(e.target.value))
                    }
                    onBlur={formik.handleBlur}
                  >
                    {formSelectOptions.locales.map((locale) => (
                      <option key={locale.id} value={locale.id}>
                        {locale.name}
                      </option>
                    ))}
                  </select>
                  {formik.errors.languageId && formik.touched.languageId ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.languageId}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>

                {/* Separator */}
                <span className="block col-span-1 md:col-span-2 lg:col-span-3 border-t border-solid border-themeColors-bgLightColor"></span>

                <div className="flex flex-col gap-2">
                  <label htmlFor="address1">{t("firstAddr")}</label>
                  <input
                    type="text"
                    id="address1"
                    name="address1"
                    className={`input ${
                      formik.errors.address1 && formik.touched.address1
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.address1}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                  />
                  {formik.errors.address1 && formik.touched.address1 ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.address1}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>
                <div className="flex flex-col gap-2">
                  <label htmlFor="address2">{t("secondAddr")}</label>
                  <input
                    type="text"
                    id="address2"
                    name="address2"
                    className={`input ${
                      formik.errors.address2 && formik.touched.address2
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.address2}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                  />
                  {formik.errors.address2 && formik.touched.address2 ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.address2}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>
                <div className="flex flex-col gap-2">
                  <label htmlFor="binLevelAlert">{t("binLevelAlert")}</label>
                  <input
                    type="number"
                    id="binLevelAlert"
                    name="binLevelAlert"
                    className={`input ${
                      formik.errors.binLevelAlert &&
                      formik.touched.binLevelAlert
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.binLevelAlert}
                    onChange={(e) =>
                      formik.setFieldValue(
                        "binLevelAlert",
                        Number(e.target.value)
                      )
                    }
                    onBlur={formik.handleBlur}
                  />
                  {formik.errors.binLevelAlert &&
                  formik.touched.binLevelAlert ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.binLevelAlert}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>

                {/* Separator */}
                <span className="block col-span-1 md:col-span-2 lg:col-span-3 border-t border-solid border-themeColors-bgLightColor"></span>

                <div className="flex flex-col gap-2">
                  <label htmlFor="city">{t("city")}</label>
                  <input
                    type="text"
                    id="city"
                    name="city"
                    className={`input ${
                      formik.errors.city && formik.touched.city
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.city}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                  />
                  {formik.errors.city && formik.touched.city ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.city}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>
                <div className="flex flex-col gap-2">
                  <label htmlFor="phoneNumber">{t("phoneNumber")}</label>
                  <input
                    type="text"
                    id="phoneNumber"
                    name="phoneNumber"
                    className={`input ${
                      formik.errors.phoneNumber && formik.touched.phoneNumber
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.phoneNumber}
                    onChange={formik.handleChange}
                    onBlur={formik.handleBlur}
                  />
                  {formik.errors.phoneNumber && formik.touched.phoneNumber ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.phoneNumber}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>
                <div className="flex flex-col gap-2">
                  <label htmlFor="countryId">{t("country")}</label>
                  <select
                    name="countryId"
                    id="countryId"
                    className={`input ${
                      formik.errors.countryId && formik.touched.countryId
                        ? "!border-themeColors-dangerColor"
                        : ""
                    }`}
                    value={formik.values.countryId}
                    onChange={(e) =>
                      formik.setFieldValue("countryId", Number(e.target.value))
                    }
                    onBlur={formik.handleBlur}
                  >
                    {formSelectOptions.countries.map((country) => (
                      <option key={country.id} value={country.id}>
                        {country.name}
                      </option>
                    ))}
                  </select>
                  {formik.errors.countryId && formik.touched.countryId ? (
                    <p className="text-xs text-themeColors-dangerColor">
                      {formik.errors.countryId}
                    </p>
                  ) : (
                    <></>
                  )}
                </div>

                {/* Separator */}
                <span className="block col-span-1 md:col-span-2 lg:col-span-3 border-t border-solid border-themeColors-bgLightColor"></span>

                <div className="flex  col-span-1 md:col-span-2 lg:col-span-3 items-center justify-end gap-3">
                  <button
                    type="button"
                    className="btn btn-secondary"
                    onClick={() => {
                      formik.resetForm();
                      handleModal("");
                    }}
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
          )}
        </div>
      </div>
    </div>
  );
};

export default UserModal;
