import {
  Button,
  CircularProgress,
  Divider,
  MenuItem,
  Select,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useForm } from "react-hook-form";
import { UserProfileData } from "../../types/UserProfileData";
import { useApi } from "../../hooks/useApi";
import axios from "axios";
import { useEffect, useState } from "react";
import { userAtom } from "../../jotai/atoms";
import { useAtom } from "jotai";
import UploadableAvatar from "../../components/UploadableAvatar/UploadableAvatar";
import SaveIcon from '@mui/icons-material/Save';
import { ImageDownloadResponse } from "../../types/ImageDownloadResponse";
import { ImageUploadData } from "../../types/ImageUploadData";
import { ImageDownloadRequest } from "../../types/ImageDownloadRequest";
import { useSnackbar } from "notistack";

function SettingsPage(){
  const [avatarFile, setAvatarFile] = useState<File | null>(null);
  const [isPhotoRemoved, setIsPhotoRemoved] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [userFieldsToAtom, setUserFieldsToAtom] = useAtom(userAtom);
  const { enqueueSnackbar } = useSnackbar();

  const {resData: recivedNewUserFields, execute: executeUserProfileData} = useApi<null, UserProfileData>(async (body) =>{
    return axios.put('/api/user', body)
  })

  const {execute: executeImageDownloadRequest} = useApi<ImageDownloadResponse, ImageDownloadRequest>(async (body) =>{
    return axios.post('/api/image/upload', body)
  })

  const { execute: executeDownloadImage } = useApi<null, ImageUploadData>(async (data) => {
    if (!data) throw new Error("Нет данных для загрузки изображения");

    return await axios.put(data.uploadUri, data.file, {
      headers: {
        "Content-Type": data.file.type
      }
    });
  });

  const {execute: executeAcceptDownload} = useApi<null, string>(async (fileId) => {
    return axios.post('/api/image/confirm', null, {
      params: { FileId: fileId },
    });
  })

  const {execute: executeDeleteProfilePhoto} = useApi(async () => {
    return axios.delete('/api/user/deleteprofilephoto');
  })

  const {register, handleSubmit, formState: { errors }} = useForm<UserProfileData>();

  const onSubmit = async (data: UserProfileData) => {
  try {
    setIsSubmitting(true);

    if (avatarFile) {
      let imageReqResponse = await executeImageDownloadRequest({
        fileName: avatarFile.name,
        mimeType: avatarFile.type,
      });

      if (!imageReqResponse?.data)
        throw new Error("Ошибка получения URL загрузки изображения " + imageReqResponse?.data);

      const { tempFileId, uploadUrl } = imageReqResponse.data;

      let downloadImageResponse = await executeDownloadImage({
        file: avatarFile,
        uploadUri: uploadUrl,
      });

      if (
        downloadImageResponse?.status !== 200 &&
        downloadImageResponse?.status !== 201
      )
        throw new Error("Ошибка загрузки изображения " + downloadImageResponse?.status);

      let acceptDownloadResponse = await executeAcceptDownload(tempFileId);

      if (
        acceptDownloadResponse?.status !== 200 &&
        acceptDownloadResponse?.status !== 201
      )
        throw new Error("Ошибка подтверждения загрузки " + acceptDownloadResponse?.status);
    } else if(isPhotoRemoved) {
      const deleteResponse = await executeDeleteProfilePhoto();
      if (deleteResponse?.status !== 200)
        throw new Error("Ошибка удаления фотографии профиля");
    }

    const payload: UserProfileData = {
      ...data,
    };

    await executeUserProfileData(payload);
    enqueueSnackbar("Save success!", { variant: 'success' })
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Unknown error';
    enqueueSnackbar(message, { variant: 'error' })
  } finally {
    setIsSubmitting(false);
  }
};

  useEffect(() => {
    if (!recivedNewUserFields) return;

    setUserFieldsToAtom(recivedNewUserFields);
  }, [recivedNewUserFields]);

  return (
    <>
      {userFieldsToAtom.id && <form onSubmit={handleSubmit(onSubmit)}>
      <Stack gap={2} fontFamily={"var(--primary-font)"}>
        <Typography variant="h4" fontWeight={700} fontFamily={"inherit"}>
          Settings
        </Typography>
        <Stack flexDirection={"row"} alignItems={"center"} gap={3}>
           <UploadableAvatar src={userFieldsToAtom.profilePhotoUri} onChange={setAvatarFile} onDelete={setIsPhotoRemoved}/>
          <Stack>
            <Typography
              variant="body1"
              fontSize={20}
              fontWeight={600}
              fontFamily={"inherit"}
            >
              {userFieldsToAtom.firstName} {userFieldsToAtom.secoundName}
            </Typography>
            <Typography
              variant="body2"
              color="gray"
              fontWeight={600}
              fontFamily={"inherit"}
            >
              {userFieldsToAtom.grade}
            </Typography>
            
          </Stack>
        </Stack>

        <Stack gap={2}>
          <Divider></Divider>
          <Typography variant="h6" fontWeight={600} fontFamily={"inherit"}>
            Basic details
          </Typography>
          <Stack flexDirection={"row"} gap={2}>
            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                Name
              </Typography>
              <TextField
                variant="outlined"
                placeholder="Jon"
                defaultValue={userFieldsToAtom.firstName}
                size="small"
                {...register("Name")}
                sx={{ width: 400 }}
              />
            </Stack>
            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                LastName
              </Typography>
              <TextField
                variant="outlined"
                placeholder="Snow"
                size="small"
                defaultValue={userFieldsToAtom.secoundName}
                {...register("LastName")}
                sx={{ width: 400 }}
              />
            </Stack>
          </Stack>
          <Stack>
            <Typography variant="body2" fontFamily={"inherit"}>
              Email
            </Typography>
            <TextField
              variant="outlined"
              placeholder="email@gmail.com"
              size="small"
              defaultValue={userFieldsToAtom.email}
              {...register("Email", {
                required: "Email is required",
                pattern: {
                  value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                  message: "Invalid email format",
                },
              })}
              sx={{ width: 400 }}
            />
            {errors.Email && (
              <Typography color="error">{errors.Email.message}</Typography>
            )}
          </Stack>
          <Stack>
            <Typography variant="body2" fontFamily={"inherit"}>
              Age
            </Typography>
            <TextField
              type="number"
              variant="outlined"
              placeholder="18"
              size="small"
              defaultValue={userFieldsToAtom.age}
              {...register("Age", {
                required: "Age is required",
                validate: (value) =>
                  value > 0 || "Age must be a positive number",
              })}
              sx={{ width: 400 }}
            />
            {errors.Age && (
              <Typography color="error">{errors.Age.message}</Typography>
            )}
          </Stack>
          <Stack>
            <Typography variant="body2" fontFamily={"inherit"}>
              Grade
            </Typography>
            <Select
              labelId="label"
              id="select"
              size="small"
              defaultValue={userFieldsToAtom.grade}
              {...register("Grade")}
              sx={{ width: 820 }}
            >
              <MenuItem value="Junior Rank">Junior Rank</MenuItem>
              <MenuItem value="Third Sports Category">
                Third Sports Category
              </MenuItem>
              <MenuItem value="Second Sports Category">
                Second Sports Category
              </MenuItem>
              <MenuItem value="First Sports Category">
                First Sports Category
              </MenuItem>
              <MenuItem value="Candidate for Master of Sport (CMS)">
                Candidate for Master of Sport (CMS)
              </MenuItem>
              <MenuItem value="Master of Sport (MS)">
                Master of Sport (MS)
              </MenuItem>
              <MenuItem value="Master of Sport, International Class (MSIC)">
                Master of Sport, International Class (MSIC)
              </MenuItem>
              <MenuItem value="Honored Master of Sport (HMS)">
                Honored Master of Sport (HMS)
              </MenuItem>
            </Select>
          </Stack>
        </Stack>

        <Stack gap={2}>
          <Divider></Divider>
          <Typography variant="h6" fontWeight={600} fontFamily={"inherit"}>
            Location
          </Typography>
          <Stack flexDirection={"row"} gap={2}>
            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                Country
              </Typography>
              <TextField
                variant="outlined"
                placeholder="Belarus"
                size="small"
                defaultValue={userFieldsToAtom.country}
                {...register("Country")}
                sx={{ width: 400 }}
              />
            </Stack>
            <Stack>
              <Typography variant="body2" fontFamily={"inherit"}>
                City
              </Typography>
              <TextField
                variant="outlined"
                placeholder="Minsk"
                size="small"
                defaultValue={userFieldsToAtom.city}
                {...register("City")}
                sx={{ width: 400 }}
              />
            </Stack>
          </Stack>

          <Stack>
            <Typography variant="body2" fontFamily={"inherit"}>
              Address
            </Typography>
            <TextField
              variant="outlined"
              placeholder="St. Sverdlova 13a"
              size="small"
              defaultValue={userFieldsToAtom.address}
              {...register("Address")}
              sx={{ width: 400 }}
            />
          </Stack>
        </Stack>
        <Stack gap={2}>
          <Divider></Divider>
          <Stack flexDirection={"row"} gap={1} justifyContent={"flex-end"}>
            {/* <Button
              variant="outlined"
              sx={{
                borderColor: "var(--accent-color)",
                color: "var(--accent-color)",
              }}
              type="submit"
            >
              Discard
            </Button> */}
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
              {isSubmitting ? "Saving..." : "Save changes"}
            </Button>
          </Stack>
        </Stack>
      </Stack>
    </form>}
    </>
  );
}

export default SettingsPage;
