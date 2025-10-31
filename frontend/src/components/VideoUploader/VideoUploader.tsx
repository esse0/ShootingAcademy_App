import React, { useState } from 'react';
import {
  Card,
  CardContent,
  Typography,
  LinearProgress,
  Box,
  Button,
  CardMedia,
} from '@mui/material';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import axios from 'axios';
import { ImageDownloadResponse } from '../../types/ImageDownloadResponse';
import { ImageDownloadRequest } from '../../types/ImageDownloadRequest';
import { useApi } from '../../hooks/useApi';
import { ImageUploadData } from '../../types/ImageUploadData';

interface Props {
  onUploaded?: (videoId: string) => void;
}

export default function VideoUploader({ onUploaded }: Props) {
  const [videoFile, setVideoFile] = useState<File | null>(null);
  const [videoPreview, setVideoPreview] = useState<string | null>(null);
  const [progress, setProgress] = useState(0);
  const [isUploading, setIsUploading] = useState(false);
  const [isComplete, setIsComplete] = useState(false);

  const {execute: executeVideoDownloadRequest} = useApi<ImageDownloadResponse, ImageDownloadRequest>(async (body) =>{
    return axios.post('/api/video/upload', body)
  })

  const { execute: executeDownloadVideo } = useApi<null, ImageUploadData>(async (data) => {
    if (!data) throw new Error("Нет данных для загрузки видео");

    return await axios.put(data.uploadUri, data.file, {
      headers: {
        "Content-Type": data.file.type
      },
      onUploadProgress: (event) => {
          const percent = Math.round((event.loaded * 100) / (event.total ?? 1));
          setProgress(percent);
      },
    });
  });

  const {execute: executeAcceptDownload} = useApi<string, string>(async (fileId) => {
    return axios.post('/api/video/confirm', null, {
      params: { FileId: fileId },
      headers: {
        "Content-Type":"application/json"
      }
    });
  })


  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file && file.type.startsWith('video/')) {
      setVideoFile(file);
      setVideoPreview(URL.createObjectURL(file));
      setProgress(0);
      setIsComplete(false);
    }
  };

  const handleUpload = async () => {
    if (!videoFile) return;
    setIsUploading(true);
    setIsComplete(false);

    try {
      // 1. Получить uploadUrl и tempFileId
      const uploadMeta = await executeVideoDownloadRequest({
        fileName: videoFile.name,
        mimeType: videoFile.type,
      })


      if (!uploadMeta?.data)
        throw new Error("Ошибка получения URL загрузки видео " + uploadMeta?.data);

      const { uploadUrl, tempFileId } = uploadMeta.data;

      // 2. Загрузить видео по URL (PUT)
      let downloadVideoResponse = await executeDownloadVideo({
        file: videoFile,
        uploadUri: uploadUrl,
      });
      if (
        downloadVideoResponse?.status !== 200 &&
        downloadVideoResponse?.status !== 201
      )
        throw new Error("Ошибка загрузки видео " + downloadVideoResponse?.status);
      
      // 3. Подтвердить загрузку
      let acceptDownloadResponse = await executeAcceptDownload(tempFileId);

      if (
        acceptDownloadResponse?.status !== 200 &&
        acceptDownloadResponse?.status !== 201 ||
        acceptDownloadResponse.data == null ||
         acceptDownloadResponse.data == ""
      )
        throw new Error("Ошибка подтверждения загрузки " + acceptDownloadResponse?.status);



      if (onUploaded) onUploaded(acceptDownloadResponse.data);
      setIsComplete(true);
    } catch (err) {
      console.error("Ошибка при загрузке видео:", err);
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <Card sx={{ width: 400, borderRadius: 2, p: 2 }}>
      <CardContent>
        

        {!videoFile && (
          <Button component="label" variant="contained" startIcon={<CloudUploadIcon />} fullWidth>
            Выбрать видео
            <input type="file" accept="video/*" hidden onChange={handleFileChange} />
          </Button>
        )}

        {videoFile && (
          <>
            {videoPreview && (
              <CardMedia component="video" src={videoPreview} controls sx={{ height: 200, mt: 2 }} />
            )}
            <Typography variant="body2" mt={2}>
              {videoFile.name}
            </Typography>

            {isUploading && (
              <Box mt={2}>
                <LinearProgress variant="determinate" value={progress} />
                <Typography mt={1}>{progress}%</Typography>
              </Box>
            )}

            {isComplete && (
              <Typography color="success.main" mt={2}>
                Загрузка завершена!
              </Typography>
            )}

            <Box display="flex" gap={1} mt={2}>
              <Button variant="contained" onClick={handleUpload} disabled={isUploading || isComplete}>
                Загрузить
              </Button>
              
              <Button
                variant="outlined"
                color="error"
                onClick={() => {
                  setVideoFile(null);
                  setVideoPreview(null);
                  setProgress(0);
                  setIsComplete(false);
                }}
              >
                Отмена
              </Button>
            </Box>
          </>
        )}
      </CardContent>
    </Card>
  );
}
