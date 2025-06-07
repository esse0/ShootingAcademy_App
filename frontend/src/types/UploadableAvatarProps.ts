import { SxProps, Theme } from "@mui/material";

export interface UploadableAvatarProps {
  sx?: SxProps<Theme>;
  src: string;
  onChange?: (file: File | null) => void;
  onDelete?: (State: boolean) => void;
}