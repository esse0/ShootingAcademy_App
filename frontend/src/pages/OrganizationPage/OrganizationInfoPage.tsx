import {
  Button,
  CircularProgress,
  Divider,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useForm } from "react-hook-form";
import { OrganizationType } from "../../types/OrganizationType";
import { useApi } from "../../hooks/useApi";
import axios from "axios";
import { useEffect, useState } from "react";
import SaveIcon from '@mui/icons-material/Save';
import { useSnackbar } from "notistack";
import { TEXTS } from '../../constants/texts';
import { useAtomValue } from "jotai";
import { userAtom } from "../../jotai/atoms";

function OrganizationInfoPage() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { enqueueSnackbar } = useSnackbar();
  const userData = useAtomValue(userAtom);

  const { execute: executeUpdateOrganization } = useApi<OrganizationType, { id: string, data: OrganizationType }>(async (params) => {
    if (!params) throw new Error(TEXTS.PARAMETERS_NOT_SPECIFIED);
    return axios.put(`/api/organization/${params.id}`, params.data)
  });

  const { resData: fetchedOrganization, execute: executeGetOrganization } = useApi<OrganizationType>(async () => {
    return axios.get('/api/user/organization')
  });

  const { register, handleSubmit, formState: { errors }, reset } = useForm<OrganizationType>({
    mode: "onChange"
  });

  useEffect(() => {
    executeGetOrganization();
  }, []);

  useEffect(() => {
    if (fetchedOrganization) {
      reset(fetchedOrganization);
    }
  }, [fetchedOrganization]);

  const onSubmit = async (data: OrganizationType) => {
    try {
      setIsSubmitting(true);
      if (!fetchedOrganization?.id) {
        throw new Error(TEXTS.ORGANIZATION_ID_NOT_FOUND);
      }
      const response = await executeUpdateOrganization({ id: fetchedOrganization.id, data });
      if (response?.data) {
        enqueueSnackbar(TEXTS.CHANGES_SAVED, { variant: 'success' });
        await executeGetOrganization();
      }
    } catch (error: any) {
      if (error?.response?.data?.ValidationErrors) {
        // Не показываем общий toast, ошибки будут под полями
      } else {
        const message = error instanceof Error ? error.message : TEXTS.UNKNOWN_ERROR;
        enqueueSnackbar(message, { variant: 'error' });
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!fetchedOrganization) {
    return (
      <Stack alignItems="center" justifyContent="center" height="100vh">
        <CircularProgress />
      </Stack>
    );
  }

  return (
    <>
      <form onSubmit={handleSubmit(onSubmit)}>
        <Stack gap={2} fontFamily={"var(--primary-font)"}>
          <input type="hidden" {...register("id")} value={fetchedOrganization?.id} />
          <Typography variant="h4" fontWeight={700} fontFamily={"inherit"}>
            {TEXTS.ORGANIZATION_SETTINGS}
          </Typography>

          <Stack gap={2}>
            <Divider />
            <Typography variant="h6" fontWeight={600} fontFamily={"inherit"}>
              {TEXTS.BASIC_INFORMATION}
            </Typography>

            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                {TEXTS.ORGANIZATION_NAME}
              </Typography>
              <TextField
                disabled={userData.role === "coach"}
                variant="outlined"
                placeholder={TEXTS.ORGANIZATION_NAME_PLACEHOLDER}
                size="small"
                {...register("name", {
                  required: "Organization name is required",
                  minLength: { value: 3, message: "Min 3 chars" },
                  maxLength: { value: 100, message: "Max 100 chars" }
                })}
                sx={{ width: 400 }}
                InputProps={{ readOnly: userData.role === "coach" }}
              />
              {errors.name && (
                <Typography color="error">{errors.name.message}</Typography>
              )}
            </Stack>

            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                {TEXTS.DESCRIPTION}
              </Typography>
              <TextField
               disabled={userData.role === "coach"}
                variant="outlined"
                placeholder={TEXTS.ORGANIZATION_DESCRIPTION_PLACEHOLDER}
                size="small"
                multiline
                rows={4}
                {...register("description", {
                  maxLength: { value: 500, message: "Max 500 chars" }
                })}
                sx={{ width: 400 }}
              />
              {errors.description && (
                <Typography color="error">{errors.description.message}</Typography>
              )}
            </Stack>

            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                {TEXTS.EMAIL}
              </Typography>
              <TextField
                disabled={userData.role === "coach"}
                variant="outlined"
                placeholder="email@example.com"
                size="small"
                {...register("email", {
                  required: "Email is required",
                  pattern: {
                    value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                    message: "Invalid email format",
                  },
                })}
                sx={{ width: 400 }}
              />
              {errors.email && (
                <Typography color="error">{errors.email.message}</Typography>
              )}
            </Stack>

            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                {TEXTS.PHONE}
              </Typography>
              <TextField
                disabled={userData.role === "coach"}
                variant="outlined"
                placeholder={TEXTS.PHONE_PLACEHOLDER}
                size="small"
                {...register("phoneNumber", {
                  required: "Phone is required",
                  pattern: {
                    value: /^\+?[0-9\s-()]{10,15}$/,
                    message: "Invalid phone format"
                  }
                })}
                sx={{ width: 400 }}
              />
              {errors.phoneNumber && (
                <Typography color="error">{errors.phoneNumber.message}</Typography>
              )}
            </Stack>
          </Stack>

          <Stack gap={2}>
            <Divider />
            <Typography variant="h6" fontWeight={600} fontFamily={"inherit"}>
              {TEXTS.ADDRESS}
            </Typography>
            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                {TEXTS.ORGANIZATION_ADDRESS}
              </Typography>
              <TextField
                variant="outlined"
                placeholder={TEXTS.ADDRESS_PLACEHOLDER}
                size="small"
                {...register("address", {
                  required: "Address is required",
                  maxLength: { value: 200, message: "Max 200 chars" }
                })}
                sx={{ width: 400 }}
              />
              {errors.address && (
                <Typography color="error">{errors.address.message}</Typography>
              )}
            </Stack>
          </Stack>

          <Stack>
            <Divider />
            {userData.role !== "coach" && (
               <Stack flexDirection={"row"} gap={1} justifyContent={"flex-end"}>
               <Button
                 sx={{ bgcolor: "var(--accent-color)" }}
                 variant="contained"
                 color="primary"
                 type="submit"
                 disabled={isSubmitting}
                 startIcon={
                   isSubmitting ? <CircularProgress size={20} color="inherit" /> : <SaveIcon />
                 }
               >
                 {isSubmitting ? TEXTS.SAVING : TEXTS.SAVE_CHANGES}
               </Button>
             </Stack>
            )}
          </Stack>
        </Stack>
      </form>
    </>
  );
}

export default OrganizationInfoPage;