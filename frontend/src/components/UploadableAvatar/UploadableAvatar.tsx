import { Avatar, Box, IconButton } from "@mui/material";
import CameraAltIcon from "@mui/icons-material/CameraAlt";
import React, { useRef, useState } from "react";
import { ALLOWED_TYPES } from "../../consts/ImageAllowedTypes";
import { UploadableAvatarProps } from "../../types/UploadableAvatarProps";
import { ClearIcon } from "@mui/x-date-pickers";

export default function UploadableAvatar({ sx, src, onChange, onDelete }: UploadableAvatarProps) {
  const [avatarSrc, setAvatarSrc] = useState<string>(src);
  const inputRef = useRef<HTMLInputElement>(null);

  const handleClick = () => {
    inputRef.current?.click();
  };

  const handleAvatarChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    if (!ALLOWED_TYPES.includes(file.type)) {
      alert("Пожалуйста, выберите изображение в формате JPG, PNG или WEBP.");
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      setAvatarSrc(reader.result as string);
      onChange?.(file);
      onDelete?.(false);
    };
    reader.readAsDataURL(file);
  };

  const handleReset = (event: React.MouseEvent) => {
    event.stopPropagation();
    inputRef.current!.value = "";
    setAvatarSrc("");
    onChange?.(null);
    if(src != "") onDelete?.(true);
  };

  return (
    <Box
      sx={{
        position: "relative",
        width: 130,
        height: 130,
        ...sx,
      }}
    >
      <Box
        onClick={handleClick}
        sx={{
          width: "100%",
          height: "100%",
          borderRadius: "50%",
          overflow: "hidden",
          position: "relative",
          cursor: "pointer",
          "&:hover .overlay": {
            opacity: 1,
            backdropFilter: "blur(4px)",
          },
        }}
      >
        <Avatar
          alt="UserAvatar"
          src={avatarSrc}
          sx={{
            width: "100%",
            height: "100%",
            objectFit: "cover",
          }}
        />
        <Box
          className="overlay"
          sx={{
            transition: "all 0.3s ease",
            opacity: 0,
            position: "absolute",
            top: 0,
            left: 0,
            width: "100%",
            height: "100%",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            color: "#fff",
            backgroundColor: "rgba(0, 0, 0, 0.3)",
            backdropFilter: "blur(0px)",
            borderRadius: "50%",
          }}
        >
          <CameraAltIcon sx={{ fontSize: 40 }} />
        </Box>
        <input
          type="file"
          accept={ALLOWED_TYPES.join(",")}
          ref={inputRef}
          onChange={handleAvatarChange}
          style={{ display: "none" }}
        />
      </Box>

       {avatarSrc && (
        <IconButton
          onClick={handleReset}
          size="small"
          sx={{
            position: "absolute",
            top: -6,
            right: -6,
            zIndex: 3,
            backgroundColor: "rgba(0,0,0,0.6)",
            color: "#fff",
            width: 28,
            height: 28,
            "&:hover": {
              backgroundColor: "rgba(0,0,0,0.8)",
            },
          }}
        >
          <ClearIcon fontSize="small" />
        </IconButton>
      )}
    </Box>
  );
}